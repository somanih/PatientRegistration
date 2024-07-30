using System;
using System.Collections.Generic;

namespace PatientRegistration.Infrastructure.DbModels;

public partial class PatientMedication
{
    public int Id { get; set; }

    public int PatientId { get; set; }

    public int? VisitId { get; set; }

    public string? MedicineName { get; set; }

    public string? Dosage { get; set; }

    public string? Frequency { get; set; }

    public string? PrescribedBy { get; set; }

    public DateOnly? PrescriptionDate { get; set; }

    public string? PrescriptionPeriod { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
