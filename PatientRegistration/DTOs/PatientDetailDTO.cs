using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PatientRegistration.DTOs;

public partial class PatientDetailDTO
{
    [Required(ErrorMessage = "First name is required.")]
    [StringLength(50, ErrorMessage = "First name cannot be longer than 50 characters.")]
    public string? FirstName { get; set; }

    [StringLength(50, ErrorMessage = "Middle name cannot be longer than 50 characters.")]
    public string? MiddleName { get; set; }

    [Required(ErrorMessage = "Last name is required.")]
    [StringLength(50, ErrorMessage = "Last name cannot be longer than 50 characters.")]
    public string? LastName { get; set; }

    [Required(ErrorMessage = "Date of birth is required.")]
    [DataType(DataType.Date)]
    public DateTime? Dob { get; set; }

    [Required(ErrorMessage = "SSN is required.")]
    [StringLength(15, ErrorMessage = "SSN cannot be longer than 15 characters.")]
    // [RegularExpression(@"^\d{1,15}$", ErrorMessage = "SSN must be a number without dashes and up to 15 digits.")]
    public string? Ssn { get; set; }

    [Required(ErrorMessage = "Address is required.")]
    public string? Address { get; set; }

    [Required(ErrorMessage = "City is required.")]
    [StringLength(80, ErrorMessage = "City cannot be longer than 80 characters.")]
    public string? City { get; set; }

    [Required(ErrorMessage = "ZIP code is required.")]
    [StringLength(10, ErrorMessage = "ZIP code cannot be longer than 10 characters.")]
    public string? Zip { get; set; }

    [Required(ErrorMessage = "State is required.")]
    [StringLength(50, ErrorMessage = "State cannot be longer than 50 characters.")]
    public string? State { get; set; }
}
