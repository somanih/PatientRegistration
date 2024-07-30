using System;
using System.Collections.Generic;

namespace PatientRegistration.Infrastructure.DbModels;

public partial class PatientVaccinationDatum
{
    public int Id { get; set; }

    public int? PatientId { get; set; }

    public string? VaccineName { get; set; }

    public DateOnly? VaccineDate { get; set; }

    public string? VaccineValidity { get; set; }

    public string? AdministeredBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
