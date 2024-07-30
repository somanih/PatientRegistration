using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging; // Import the ILogger namespace
using PatientRegistration.DTOs;
using PatientRegistration.Infrastructure.DbModels;
using PatientRegistration.Services.Interfaces;

namespace PatientRegistration.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly IPatientService _patientService;
        private readonly ILogger<PatientController> _logger; 

        public PatientController(IPatientService patientService, ILogger<PatientController> logger)
        {
            _patientService = patientService;
            _logger = logger; 
        }

        [HttpGet("isExistingPatient")]
        public async Task<IActionResult> IsExistingPatient([FromQuery] string ssn)
        {
            try
            {
                _logger.LogInformation("Checking if patient exists with SSN: {SSN}", ssn);
                var exists = await _patientService.IsExistingPatientAsync(ssn);
                if (!exists)
                {
                    _logger.LogInformation("Patient not found with SSN: {SSN}", ssn);
                    return NotFound("Patient not found.");
                }

                var patient = await _patientService.GetPatientDataAsync(ssn);
                _logger.LogInformation("Patient found with ID: {PatientId}", patient.Id);
                return Ok(new { patientId = patient.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while checking if patient exists with SSN: {SSN}", ssn);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        [HttpGet("getPatientData/{patientId}")]
        public async Task<IActionResult> GetPatientData(int patientId)
        {
            try
            {
                _logger.LogInformation("Fetching data for patient with ID: {PatientId}", patientId);
                var patient = await _patientService.GetPatientDataByIdAsync(patientId);
                if (patient == null)
                {
                    _logger.LogInformation("Patient not found with ID: {PatientId}", patientId);
                    return NotFound("Patient not found.");
                }

                _logger.LogInformation("Patient data retrieved for ID: {PatientId}", patientId);
                return Ok(patient);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching patient data for ID: {PatientId}", patientId);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        [HttpPost("registerNewPatient")]
        public async Task<IActionResult> RegisterNewPatient([FromBody] PatientDetailDTO patient)
        {
            try
            {
                _logger.LogInformation("Registering new patient with SSN: {SSN}", patient.Ssn);
                if (await _patientService.IsExistingPatientAsync(patient.Ssn))
                {
                    _logger.LogInformation("Patient already exists with ssn: {ssn}", patient.Ssn);
                    return Conflict("Patient already exists.");
                }

                var patientId =  await _patientService.RegisterNewPatientAsync(patient);
                await _patientService.FetchPatientDataInBackgroundAsync(patient.Ssn, patientId);

                _logger.LogInformation("New patient registered with ID: {PatientId}", patientId);
                return CreatedAtAction(nameof(GetPatientData), new { patientId }, new { patientId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while registering new patient with SSN: {SSN}", patient.Ssn);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }
    }
}
