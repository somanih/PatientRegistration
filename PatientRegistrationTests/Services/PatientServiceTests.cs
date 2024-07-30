using AutoMapper;
using Moq;
using Moq.EntityFrameworkCore;
using PatientRegistration.DTOs;
using PatientRegistration.Infrastructure.Configuration;
using PatientRegistration.Infrastructure.DbModels;
using PatientRegistration.Services.ExternalModels;
using PatientRegistration.Services.Implementation;
using System.Collections.Generic;
using System.Net.Http.Json;
using RichardSzalay.MockHttp;
using System.Threading.Tasks;
using Xunit;
using System.Net;
using System.Text;
using Microsoft.Extensions.Options;
using PatientRegistration.Services.Configuration;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using PatientRegistrationTests.MockData;
using Microsoft.Extensions.Logging;

namespace PatientRegistration.Tests
{
    public class PatientServiceTests
    {
        private readonly Mock<PatientDbContext> _mockContext;
        private readonly Mock<IHttpClientFactory> _mockHttpClientFactory;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<PatientService>> _mockLogger;
        private readonly PatientService _service;
        private readonly Mock<IOptions<ApiSettings>> _mockApiSettings;

        public PatientServiceTests()
        {
            _mockContext = new Mock<PatientDbContext>();
            _mockHttpClientFactory = new Mock<IHttpClientFactory>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<PatientService>>();

            var apiSettings = new ApiSettings
            {
                AuthUrl = "https://testapi.mindware.us/auth/local",
                PatientLabVisitsUrl = "https://testapi.mindware.us/patient-lab-visits",
                PatientLabResultsUrl = "https://testapi.mindware.us/Patient-lab-results",
                PatientMedicationsUrl = "https://testapi.mindware.us/patient-medications",
                PatientVaccinationsUrl = "https://testapi.mindware.us/patient-vaccinations"
            };
            _mockApiSettings = new Mock<IOptions<ApiSettings>>();
            _mockApiSettings.Setup(x => x.Value).Returns(apiSettings);

            _service = new PatientService(_mockContext.Object, _mockHttpClientFactory.Object, _mockMapper.Object, _mockApiSettings.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task IsExistingPatientAsync_PatientExists_ReturnsTrue()
        {
            // Arrange
            var ssn = "123-45-6789";
            var patientDetails = MockDataGenerator.GetMockPatientDetails();

            _mockContext
                .Setup(c => c.PatientDetails)
                .ReturnsDbSet(patientDetails);

            // Act
            var result = await _service.IsExistingPatientAsync(ssn);

            // Assert
            Assert.True(result);
            _mockLogger.Verify(l => l.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Checking if patient with SSN 123-45-6789 exists.")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()
            ), Times.Once);
        }

        [Fact]
        public async Task GetPatientDataByIdAsync_PatientExists_ReturnsPatientDetail()
        {
            // Arrange
            var patientId = 1;
            var patientDetail = MockDataGenerator.GetMockPatientDetail();

            _mockContext
                .Setup(c => c.PatientDetails.FindAsync(patientId))
                .ReturnsAsync(patientDetail);

            // Act
            var result = await _service.GetPatientDataByIdAsync(patientId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(patientId, result.Id);
            _mockLogger.Verify(l => l.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Fetching patient data for ID {patientId}.")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()
            ), Times.Once);
        }

        [Fact]
        public async Task RegisterNewPatientAsync_AddsPatientToContext()
        {
            // Arrange
            var patientDto = new PatientDetailDTO { };
            var patientDetail = MockDataGenerator.GetMockPatientDetail();

            _mockMapper
                .Setup(m => m.Map<PatientDetail>(patientDto))
                .Returns(patientDetail);
            _mockContext
                .Setup(c => c.PatientDetails.Add(patientDetail));

            // Act
            await _service.RegisterNewPatientAsync(patientDto);

            // Assert
            _mockContext.Verify(c => c.PatientDetails.Add(It.IsAny<PatientDetail>()), Times.Once);
            _mockLogger.Verify(l => l.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Registering new patient with data {patientDto}.")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()
            ), Times.Once);
        }

        [Fact]
        public async Task FetchPatientDataInBackgroundAsync_FetchesDataSuccessfully()
        {
            // Arrange
            var ssn = "123-45-6789";
            var patientId = 1;
            var patientMedication = MockDataGenerator.GetMockPatientMedication();
            var patientLabResult = MockDataGenerator.GetMockPatientLabResult();
            var patientLabVisit = MockDataGenerator.GetMockPatientLabVisit();
            var patientVaccinationDatum = MockDataGenerator.GetMockPatientVaccinationDatum();

            _mockContext
                .Setup(c => c.PatientMedications.Add(patientMedication));
            _mockContext
                .Setup(c => c.PatientLabResults.Add(patientLabResult));
            _mockContext
                .Setup(c => c.PatientLabVisits.Add(patientLabVisit));
            _mockContext
                .Setup(c => c.PatientVaccinationData.Add(patientVaccinationDatum));

            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When("https://testapi.mindware.us/auth/local")
                .Respond(req => new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{\"jwt\": \"test-token\"}", Encoding.UTF8, "application/json")
                });

            mockHttp.When($"https://testapi.mindware.us/patient-lab-visits?SSN={ssn}")
                .Respond(req => new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = JsonContent.Create(new List<LabVisit>
                    {
                        new LabVisit { Id = 1, LabName = "Lab1", LabTestRequest = "Test1", ResultDate = DateTime.UtcNow }
                    })
                });

            mockHttp.When($"https://testapi.mindware.us/Patient-lab-results/?lab_visit_id=1")
                .Respond(req => new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = JsonContent.Create(new List<LabResult>
                    {
                        new LabResult { TestName = "Test1", TestResult = "Positive", TestObservation = "Observation1", Attachments = new List<object>() }
                    })
                });

            mockHttp.When($"https://testapi.mindware.us/patient-medications?SSN={ssn}")
                .Respond(req => new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = JsonContent.Create(new List<Medication>
                    {
                        new Medication { VisitId = 1, MedicineName = "Med1", Dosage = "Dosage1", Frequency = "Once a day", PrescribedBy = "Doctor1", PrescriptionDate = DateTime.UtcNow, PrescriptionPeriod = "1 Week" }
                    })
                });

            mockHttp.When($"https://testapi.mindware.us/patient-vaccinations?SSN={ssn}")
                .Respond(req => new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = JsonContent.Create(new List<Vaccination>
                    {
                        new Vaccination { VaccineName = "Vaccine1", VaccineDate = DateTime.UtcNow, VaccineValidity = "1 Year", AdministeredBy = "Nurse1" }
                    })
                });

            var client = mockHttp.ToHttpClient();
            _mockHttpClientFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(client);

            // Act
            await _service.FetchPatientDataInBackgroundAsync(ssn, patientId);

            // Assert
            _mockContext.Verify(c => c.PatientLabVisits.Add(It.IsAny<PatientLabVisit>()), Times.Once);
            _mockContext.Verify(c => c.PatientLabResults.Add(It.IsAny<PatientLabResult>()), Times.Once);
            _mockContext.Verify(c => c.PatientMedications.Add(It.IsAny<PatientMedication>()), Times.Once);
            _mockContext.Verify(c => c.PatientVaccinationData.Add(It.IsAny<PatientVaccinationDatum>()), Times.Once);
            _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));
            _mockLogger.Verify(l => l.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Starting background fetch for patient data with SSN {ssn} and ID {patientId}.")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()
            ), Times.Once);
            _mockLogger.Verify(l => l.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Completed fetching patient data for SSN {ssn} and ID {patientId}.")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()
            ), Times.Once);
        }
    }
}
