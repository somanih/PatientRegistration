using System;
using System.Collections.Generic;

namespace PatientRegistration.Infrastructure.DbModels;

public partial class PatientDetail
{
    public int Id { get; set; }

    public string? FirstName { get; set; }

    public string? MiddleName { get; set; }

    public string? LastName { get; set; }

    public DateTime? Dob { get; set; }

    public string? Ssn { get; set; }

    public string? Address { get; set; }

    public string? City { get; set; }

    public string? Zip { get; set; }

    public string? State { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
