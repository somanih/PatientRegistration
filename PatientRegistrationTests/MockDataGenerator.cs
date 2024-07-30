using System;
using System.Collections.Generic;
using PatientRegistration.Infrastructure.DbModels;

namespace PatientRegistrationTests.MockData
{
    public static class MockDataGenerator
    {
        public static PatientDetail GetMockPatientDetail()
        {
            return new PatientDetail
            {
                Id = 1,
                FirstName = "John",
                MiddleName = "A.",
                LastName = "Doe",
                Dob = new DateTime(1990, 1, 1),
                Ssn = "123-45-6789",
                Address = "123 Main St",
                City = "Anytown",
                Zip = "12345",
                State = "State",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }

        public static List<PatientDetail> GetMockPatientDetails()
        {
            return new List<PatientDetail>
            {
                GetMockPatientDetail(),
                new PatientDetail
                {
                    Id = 2,
                    FirstName = "Jane",
                    LastName = "Smith",
                    Dob = new DateTime(1985, 5, 15),
                    Ssn = "987-65-4321",
                    Address = "456 Elm St",
                    City = "Othertown",
                    Zip = "67890",
                    State = "OtherState",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };
        }

        public static PatientLabResult GetMockPatientLabResult()
        {
            return new PatientLabResult
            {
                Id = 1,
                LabVisitId = 1,
                TestName = "Blood Test",
                TestResult = "Positive",
                TestObservation = "Observation",
                Attachments = "None",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }

        public static PatientLabVisit GetMockPatientLabVisit()
        {
            return new PatientLabVisit
            {
                Id = 1,
                PatientId = 1,
                LabName = "LabCorp",
                LabTestRequest = "Blood Test",
                ResultDate = DateTime.UtcNow.ToShortDateString(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }

        public static PatientMedication GetMockPatientMedication()
        {
            return new PatientMedication
            {
                Id = 1,
                PatientId = 1,
                VisitId = 1,
                MedicineName = "Aspirin",
                Dosage = "100mg",
                Frequency = "Once a day",
                PrescribedBy = "Dr. John Doe",
                PrescriptionDate = DateOnly.FromDateTime(DateTime.UtcNow),
                PrescriptionPeriod = "7 days",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }

        public static PatientVaccinationDatum GetMockPatientVaccinationDatum()
        {
            return new PatientVaccinationDatum
            {
                Id = 1,
                PatientId = 1,
                VaccineName = "COVID-19",
                VaccineDate = DateOnly.FromDateTime(DateTime.UtcNow),
                VaccineValidity = "1 year",
                AdministeredBy = "Nurse Jane",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }
    }
}
