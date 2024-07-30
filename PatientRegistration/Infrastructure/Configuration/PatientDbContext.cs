using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using PatientRegistration.Infrastructure.DbModels;

namespace PatientRegistration.Infrastructure.Configuration;

public partial class PatientDbContext : DbContext
{
    public PatientDbContext()
    {
    }

    public PatientDbContext(DbContextOptions<PatientDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<PatientDetail> PatientDetails { get; set; }

    public virtual DbSet<PatientLabResult> PatientLabResults { get; set; }

    public virtual DbSet<PatientLabVisit> PatientLabVisits { get; set; }

    public virtual DbSet<PatientMedication> PatientMedications { get; set; }

    public virtual DbSet<PatientVaccinationDatum> PatientVaccinationData { get; set; }

    public virtual DbSet<PatientVisitHistory> PatientVisitHistories { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-BVEKDVS\\SQLEXPRESS;Database=PatientDB;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PatientDetail>(entity =>
        {
            entity.ToTable("Patient_Details");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Address).HasColumnName("address");
            entity.Property(e => e.City)
                .HasMaxLength(80)
                .HasColumnName("city");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Dob).HasColumnName("dob");
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .HasColumnName("first_name");
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .HasColumnName("last_name");
            entity.Property(e => e.MiddleName)
                .HasMaxLength(50)
                .HasColumnName("middle_name");
            entity.Property(e => e.Ssn)
                .HasMaxLength(15)
                .HasColumnName("ssn");
            entity.Property(e => e.State)
                .HasMaxLength(50)
                .HasColumnName("state");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.Zip)
                .HasMaxLength(10)
                .HasColumnName("zip");
        });

        modelBuilder.Entity<PatientLabResult>(entity =>
        {
            entity.ToTable("Patient_Lab_Result");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Attachments)
                .HasMaxLength(50)
                .HasColumnName("attachments");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.LabVisitId).HasColumnName("lab_visit_id");
            entity.Property(e => e.TestName)
                .HasMaxLength(50)
                .HasColumnName("test_name");
            entity.Property(e => e.TestObservation)
                .HasMaxLength(50)
                .HasColumnName("test_observation");
            entity.Property(e => e.TestResult)
                .HasMaxLength(50)
                .HasColumnName("test_result");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<PatientLabVisit>(entity =>
        {
            entity.ToTable("Patient_Lab_Visit");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.LabName)
                .HasMaxLength(50)
                .HasColumnName("lab_name");
            entity.Property(e => e.LabTestRequest)
                .HasMaxLength(100)
                .HasColumnName("lab_test_request");
            entity.Property(e => e.PatientId).HasColumnName("patient_id");
            entity.Property(e => e.ResultDate)
                .HasMaxLength(50)
                .HasColumnName("result_date");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<PatientMedication>(entity =>
        {
            entity.ToTable("Patient_Medication");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Dosage)
                .HasMaxLength(50)
                .HasColumnName("dosage");
            entity.Property(e => e.Frequency)
                .HasMaxLength(50)
                .HasColumnName("frequency");
            entity.Property(e => e.MedicineName)
                .HasMaxLength(50)
                .HasColumnName("medicine_name");
            entity.Property(e => e.PatientId).HasColumnName("Patient_id");
            entity.Property(e => e.PrescribedBy)
                .HasMaxLength(50)
                .HasColumnName("Prescribed_by");
            entity.Property(e => e.PrescriptionDate).HasColumnName("Prescription_date");
            entity.Property(e => e.PrescriptionPeriod)
                .HasMaxLength(50)
                .HasColumnName("Prescription_period");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.VisitId).HasColumnName("visit_id");
        });

        modelBuilder.Entity<PatientVaccinationDatum>(entity =>
        {
            entity.ToTable("Patient_Vaccination_Data");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AdministeredBy)
                .HasMaxLength(50)
                .HasColumnName("administered_by");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.PatientId).HasColumnName("patient_id");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.VaccineDate).HasColumnName("vaccine_date");
            entity.Property(e => e.VaccineName)
                .HasMaxLength(50)
                .HasColumnName("vaccine_name");
            entity.Property(e => e.VaccineValidity)
                .HasMaxLength(50)
                .HasColumnName("vaccine_validity");
        });

        modelBuilder.Entity<PatientVisitHistory>(entity =>
        {
            entity.ToTable("Patient_Visit_History");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.DoctorName)
                .HasMaxLength(50)
                .HasColumnName("doctor_name");
            entity.Property(e => e.NurseName1)
                .HasMaxLength(50)
                .HasColumnName("nurse_name_1");
            entity.Property(e => e.NurseName2)
                .HasMaxLength(50)
                .HasColumnName("nurse_name_2");
            entity.Property(e => e.PatientId).HasColumnName("patient_id");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.VisitDate)
                .HasColumnType("datetime")
                .HasColumnName("visit_date");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
