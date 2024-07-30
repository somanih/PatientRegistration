using AutoMapper;
using Microsoft.AspNetCore.SignalR.Protocol;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PatientRegistration.DTOs;
using PatientRegistration.Infrastructure.Configuration;
using PatientRegistration.Infrastructure.DbModels;
using PatientRegistration.Services.Configuration;
using PatientRegistration.Services.ExternalModels;
using PatientRegistration.Services.Interfaces;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace PatientRegistration.Services.Implementation
{
    public class PatientService : IPatientService
    {
        private readonly PatientDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMapper _mapper;
        private readonly ApiSettings _apiSettings;
        private readonly ILogger<PatientService> _logger;

        public PatientService(PatientDbContext context, IHttpClientFactory httpClientFactory, IMapper mapper, IOptions<ApiSettings> apiSettings, ILogger<PatientService> logger)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
            _mapper = mapper;
            _apiSettings = apiSettings.Value;
            _logger = logger;
        }

        public async Task<bool> IsExistingPatientAsync(string ssn)
        {
            _logger.LogInformation("Checking if patient with SSN {Ssn} exists.", ssn);
            return await _context.PatientDetails.AnyAsync(p => p.Ssn == ssn);
        }

        public async Task<PatientDetail> GetPatientDataAsync(string ssn)
        {
            _logger.LogInformation("Fetching patient data for SSN {Ssn}.", ssn);
            return await _context.PatientDetails.FirstOrDefaultAsync(x => x.Ssn.Equals(ssn));
        }

        public async Task<PatientDetail> GetPatientDataByIdAsync(int patientId)
        {
            _logger.LogInformation("Fetching patient data for ID {PatientId}.", patientId);
            return await _context.PatientDetails.FindAsync(patientId);
        }

        public async Task<int> RegisterNewPatientAsync(PatientDetailDTO patient)
        {
            _logger.LogInformation("Registering new patient with data {@Patient}.", patient);
            var patientDb = _mapper.Map<PatientDetail>(patient);

            patientDb.CreatedAt = DateTime.UtcNow;
            patientDb.UpdatedAt = DateTime.UtcNow;

            _context.PatientDetails.Add(patientDb);
            await _context.SaveChangesAsync();
            return patientDb.Id;
        }

        public async Task FetchPatientDataInBackgroundAsync(string ssn, int patientId)
        {
            _logger.LogInformation("Starting background fetch for patient data with SSN {Ssn} and ID {PatientId}.", ssn, patientId);

            var client = _httpClientFactory.CreateClient();
            var authResponse = await client.PostAsync(
                _apiSettings.AuthUrl,
                new StringContent("{\"identifier\": \"user1@test.com\", \"password\": \"Test123!\"}", Encoding.UTF8, "application/json")
            );

            if (!authResponse.IsSuccessStatusCode)
            {
                _logger.LogError("Authentication request failed with status code {StatusCode}.", authResponse.StatusCode);
                return;
            }

            var authResponseContent = await authResponse.Content.ReadAsStringAsync();
            var authToken = ExtractTokenFromResponse(authResponseContent);

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
            // var stopWatch = Stopwatch.StartNew();

            var tasks = new[]
            {
                FetchLabDataAsync(client, ssn, patientId),
                FetchMedicationDataAsync(client, ssn, patientId),
                FetchVaccinationDataAsync(client, ssn, patientId)
            };

            await Task.WhenAll(tasks);
            // _logger.LogInformation($"Time : {stopWatch.ElapsedMilliseconds}");
            _logger.LogInformation("Completed fetching patient data for SSN {Ssn} and ID {PatientId}.", ssn, patientId);
            await ConcurrencyHandler.SaveChangesWithConcurrencyHandlingAsync(_context);
        }

        #region Private Methods

        private string ExtractTokenFromResponse(string responseContent)
        {
            using var jsonDoc = JsonDocument.Parse(responseContent);
            var root = jsonDoc.RootElement;
            if (root.TryGetProperty("jwt", out var jwtElement))
            {
                return jwtElement.GetString();
            }
            throw new Exception("JWT token not found in the response.");
        }

        private async Task FetchLabDataAsync(HttpClient client, string ssn, int patientId)
        {
            _logger.LogInformation("Fetching lab data for SSN {Ssn}.", ssn);

            var response = await client.GetAsync($"{_apiSettings.PatientLabVisitsUrl}?SSN={ssn}");
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Lab data request failed with status code {StatusCode}.", response.StatusCode);
                return;
            }

            var labVisits = await response.Content.ReadFromJsonAsync<List<LabVisit>>();

            foreach (var labVisit in labVisits)
            {
                var labVisitEntity = new PatientLabVisit
                {
                    PatientId = patientId,
                    LabName = labVisit.LabName,
                    LabTestRequest = labVisit.LabTestRequest,
                    ResultDate = labVisit.ResultDate.ToShortDateString(),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.PatientLabVisits.Add(labVisitEntity);
                await ConcurrencyHandler.SaveChangesWithConcurrencyHandlingAsync(_context);

                var labResultsResponse = await client.GetAsync($"{_apiSettings.PatientLabResultsUrl}/?lab_visit_id={labVisit.Id}");
                if (!labResultsResponse.IsSuccessStatusCode)
                {
                    _logger.LogError("Lab results request failed with status code {StatusCode}.", labResultsResponse.StatusCode);
                    continue;
                }

                var labResults = await labResultsResponse.Content.ReadFromJsonAsync<List<LabResult>>();

                foreach (var labResult in labResults)
                {
                    var labResultEntity = new PatientLabResult
                    {
                        LabVisitId = labVisitEntity.Id,
                        TestName = labResult.TestName,
                        TestResult = labResult.TestResult,
                        TestObservation = labResult.TestObservation,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    _context.PatientLabResults.Add(labResultEntity);
                }
            }
        }

        private async Task FetchMedicationDataAsync(HttpClient client, string ssn, int patientId)
        {
            _logger.LogInformation("Fetching medication data for SSN {Ssn}.", ssn);

            var response = await client.GetAsync($"{_apiSettings.PatientMedicationsUrl}?SSN={ssn}");
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Medication data request failed with status code {StatusCode}.", response.StatusCode);
                return;
            }

            var medications = await response.Content.ReadFromJsonAsync<List<Medication>>();

            foreach (var medication in medications)
            {
                var medicationEntity = new PatientMedication
                {
                    PatientId = patientId,
                    VisitId = medication.VisitId,
                    MedicineName = medication.MedicineName,
                    Dosage = medication.Dosage,
                    Frequency = medication.Frequency,
                    PrescribedBy = medication.PrescribedBy,
                    PrescriptionDate = DateOnly.FromDateTime(medication.PrescriptionDate),
                    PrescriptionPeriod = medication.PrescriptionPeriod,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.PatientMedications.Add(medicationEntity);
            }
        }

        private async Task FetchVaccinationDataAsync(HttpClient client, string ssn, int patientId)
        {
            _logger.LogInformation("Fetching vaccination data for SSN {Ssn}.", ssn);

            var response = await client.GetAsync($"{_apiSettings.PatientVaccinationsUrl}?SSN={ssn}");
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Vaccination data request failed with status code {StatusCode}.", response.StatusCode);
                return;
            }

            var vaccinations = await response.Content.ReadFromJsonAsync<List<Vaccination>>();

            foreach (var vaccination in vaccinations)
            {
                var vaccinationEntity = new PatientVaccinationDatum
                {
                    PatientId = patientId,
                    VaccineName = vaccination.VaccineName,
                    VaccineDate = DateOnly.FromDateTime(vaccination.VaccineDate),
                    VaccineValidity = vaccination.VaccineValidity,
                    AdministeredBy = vaccination.AdministeredBy,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.PatientVaccinationData.Add(vaccinationEntity);
            }
        }

        #endregion
    }
}
