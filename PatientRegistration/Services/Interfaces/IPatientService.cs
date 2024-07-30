using PatientRegistration.DTOs;
using PatientRegistration.Infrastructure.DbModels;

namespace PatientRegistration.Services.Interfaces
{
    public interface IPatientService
    {
        Task<bool> IsExistingPatientAsync(string ssn);
        Task<PatientDetail> GetPatientDataAsync(string ssn);
        Task<PatientDetail> GetPatientDataByIdAsync(int patienId);
        Task<int> RegisterNewPatientAsync(PatientDetailDTO patient);
        Task FetchPatientDataInBackgroundAsync(string ssn, int patientId);
    }
}
