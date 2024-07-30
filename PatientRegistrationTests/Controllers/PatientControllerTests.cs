using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using PatientRegistration.Controllers;
using PatientRegistration.DTOs;
using PatientRegistration.Infrastructure.DbModels;
using PatientRegistration.Services.Interfaces;
using PatientRegistrationTests.MockData;
using System.Runtime.Intrinsics.X86;

public class PatientControllerTests
{
    private readonly Mock<IPatientService> _mockPatientService;
    private readonly Mock<ILogger<PatientController>> _mockLogger;
    private readonly PatientController _controller;

    public PatientControllerTests()
    {
        _mockPatientService = new Mock<IPatientService>();
        _mockLogger = new Mock<ILogger<PatientController>>();
        _controller = new PatientController(_mockPatientService.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task IsExistingPatient_PatientExists_ReturnsOk()
    {
        // Arrange
        var ssn = "123456789";
        var patientId = 1;
        _mockPatientService.Setup(s => s.IsExistingPatientAsync(ssn)).ReturnsAsync(true);
        _mockPatientService.Setup(s => s.GetPatientDataAsync(It.IsAny<string>())).ReturnsAsync(MockDataGenerator.GetMockPatientDetail());

        // Act
        var result = await _controller.IsExistingPatient(ssn) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
    }

    [Fact]
    public async Task IsExistingPatient_PatientDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var ssn = "123-45-6789";
        _mockPatientService.Setup(s => s.IsExistingPatientAsync(ssn)).ReturnsAsync(false);

        // Act
        var result = await _controller.IsExistingPatient(ssn) as NotFoundObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
        Assert.Equal("Patient not found.", result.Value);
    }

    [Fact]
    public async Task IsExistingPatient_ExceptionThrown_ReturnsInternalServerError()
    {
        // Arrange
        var ssn = "123-45-6789";
        _mockPatientService.Setup(s => s.IsExistingPatientAsync(ssn)).ThrowsAsync(new Exception("Test exception"));

        // Act
        var result = await _controller.IsExistingPatient(ssn) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status500InternalServerError, result.StatusCode);
        Assert.Equal("An error occurred while processing your request.", result.Value);
    }

    [Fact]
    public async Task GetPatientData_PatientExists_ReturnsOk()
    {
        // Arrange
        var patientId = 1;
        var patientDetail = MockDataGenerator.GetMockPatientDetail();
        _mockPatientService.Setup(s => s.GetPatientDataByIdAsync(It.IsAny<int>())).ReturnsAsync(patientDetail);

        // Act
        var result = await _controller.GetPatientData(patientId) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        Assert.Equal(patientDetail, result.Value);
    }

    [Fact]
    public async Task GetPatientData_PatientDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var patientId = 1;
        _mockPatientService.Setup(s => s.GetPatientDataAsync(It.IsAny<string>())).ReturnsAsync((PatientDetail)null);

        // Act
        var result = await _controller.GetPatientData(patientId) as NotFoundObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
        Assert.Equal("Patient not found.", result.Value);
    }

    [Fact]
    public async Task GetPatientData_ExceptionThrown_ReturnsInternalServerError()
    {
        // Arrange
        var patientId = 1;
        _mockPatientService.Setup(s => s.GetPatientDataByIdAsync(It.IsAny<int>())).ThrowsAsync(new Exception("Test exception"));

        // Act
        var result = await _controller.GetPatientData(patientId) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status500InternalServerError, result.StatusCode);
        Assert.Equal("An error occurred while processing your request.", result.Value);
    }

    [Fact]
    public async Task RegisterNewPatient_PatientDoesNotExist_ReturnsCreated()
    {
        // Arrange
        var patientDto = new PatientDetailDTO { Ssn = "12345789" };
        var patientId = 1;
        _mockPatientService.Setup(s => s.IsExistingPatientAsync(patientDto.Ssn)).ReturnsAsync(false);
        _mockPatientService.Setup(s => s.RegisterNewPatientAsync(patientDto)).ReturnsAsync(patientId);

        // Act
        var result = await _controller.RegisterNewPatient(patientDto) as CreatedAtActionResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status201Created, result.StatusCode);
        Assert.Equal(nameof(PatientController.GetPatientData), result.ActionName);
    }

    [Fact]
    public async Task RegisterNewPatient_PatientExists_ReturnsConflict()
    {
        // Arrange
        var patientDto = new PatientDetailDTO { Ssn = "123-45-6789" };
        _mockPatientService.Setup(s => s.IsExistingPatientAsync(patientDto.Ssn)).ReturnsAsync(true);

        // Act
        var result = await _controller.RegisterNewPatient(patientDto) as ConflictObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status409Conflict, result.StatusCode);
        Assert.Equal("Patient already exists.", result.Value);
    }

    [Fact]
    public async Task RegisterNewPatient_ExceptionThrown_ReturnsInternalServerError()
    {
        // Arrange
        var patientDto = new PatientDetailDTO { Ssn = "123-45-6789" };
        _mockPatientService.Setup(s => s.IsExistingPatientAsync(patientDto.Ssn)).ThrowsAsync(new Exception("Test exception"));

        // Act
        var result = await _controller.RegisterNewPatient(patientDto) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status500InternalServerError, result.StatusCode);
        Assert.Equal("An error occurred while processing your request.", result.Value);
    }
}
