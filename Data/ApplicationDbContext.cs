using ClinicalView.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ClinicalView.Data;

// The DbContext is the bridge between C# and the database.
// IdentityDbContext already contains the login tables (AspNetUsers, ...). We add our clinical tables.
public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // Each DbSet is one table we can query with C# (LINQ).
    public DbSet<Patient> Patients { get; set; } = default!;
    public DbSet<Allergy> Allergies { get; set; } = default!;
    public DbSet<Medication> Medications { get; set; } = default!;
    public DbSet<PatientAlert> PatientAlerts { get; set; } = default!;
    public DbSet<PatientDocument> PatientDocuments { get; set; } = default!;

    // Fine-tune table names, column sizes, keys, indexes and relationships.
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder); // Required: builds the Identity (login) tables.

        builder.Entity<Patient>(entity =>
        {
            entity.ToTable("Patient");
            entity.HasKey(p => p.PatientId);
            entity.Property(p => p.MRN).HasMaxLength(20).IsRequired();
            entity.Property(p => p.FirstName).HasMaxLength(50).IsRequired();
            entity.Property(p => p.LastName).HasMaxLength(50).IsRequired();
            entity.Property(p => p.Gender).HasMaxLength(10);
            entity.Property(p => p.PhoneNumber).HasMaxLength(20);
            entity.Property(p => p.AddressLine1).HasMaxLength(100);
            entity.Property(p => p.City).HasMaxLength(50);
            entity.Property(p => p.State).HasMaxLength(20);
            entity.Property(p => p.ZipCode).HasMaxLength(10);

            // Indexes make searches fast. MRN must be unique.
            entity.HasIndex(p => p.MRN).IsUnique();
            entity.HasIndex(p => new { p.LastName, p.FirstName });
            entity.HasIndex(p => p.DOB);
        });

        builder.Entity<Allergy>(entity =>
        {
            entity.ToTable("Allergy");
            entity.HasKey(a => a.AllergyId);
            entity.Property(a => a.AllergyName).HasMaxLength(100).IsRequired();
            entity.Property(a => a.Severity).HasMaxLength(20);
            entity.Property(a => a.Status).HasMaxLength(20);
            entity.HasOne(a => a.Patient)
                  .WithMany(p => p.Allergies)
                  .HasForeignKey(a => a.PatientId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<Medication>(entity =>
        {
            entity.ToTable("Medication");
            entity.HasKey(m => m.MedicationId);
            entity.Property(m => m.MedicationName).HasMaxLength(100).IsRequired();
            entity.Property(m => m.Strength).HasMaxLength(50);
            entity.Property(m => m.DosageInstructions).HasMaxLength(200);
            entity.Property(m => m.Frequency).HasMaxLength(50);
            entity.Property(m => m.Route).HasMaxLength(50);
            entity.Property(m => m.PrescribingProvider).HasMaxLength(100);
            entity.Property(m => m.Status).HasMaxLength(20);
            entity.HasOne(m => m.Patient)
                  .WithMany(p => p.Medications)
                  .HasForeignKey(m => m.PatientId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<PatientAlert>(entity =>
        {
            entity.ToTable("Alert");
            entity.HasKey(a => a.AlertId);
            entity.Property(a => a.AlertType).HasMaxLength(50).IsRequired();
            entity.Property(a => a.Description).HasMaxLength(200);
            entity.Property(a => a.Severity).HasMaxLength(20);
            entity.Property(a => a.Status).HasMaxLength(20);
            entity.HasOne(a => a.Patient)
                  .WithMany(p => p.Alerts)
                  .HasForeignKey(a => a.PatientId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<PatientDocument>(entity =>
        {
            entity.ToTable("Document");
            entity.HasKey(d => d.DocumentId);
            entity.Property(d => d.DocumentName).HasMaxLength(200).IsRequired();
            entity.Property(d => d.DocumentType).HasMaxLength(50);
            entity.Property(d => d.FilePath).HasMaxLength(500).IsRequired();
            entity.HasOne(d => d.Patient)
                  .WithMany(p => p.Documents)
                  .HasForeignKey(d => d.PatientId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}