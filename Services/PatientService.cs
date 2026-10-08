using ClinicalView.Data;
using ClinicalView.Dtos;
using ClinicalView.Models;
using Microsoft.EntityFrameworkCore;

namespace ClinicalView.Services;

// The class says HOW: it uses EF Core (ApplicationDbContext) to read the database.
public class PatientService : IPatientService
{
    private readonly ApplicationDbContext _db;

    // Dependency injection: ASP.NET Core passes in the DbContext automatically.
    public PatientService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<List<PatientDto>> SearchAsync(PatientSearchRequest request)
    {
        // Start with "all patients" and add a filter for each field the user filled in.
        // Nothing runs in the database until ToListAsync() at the end.
        IQueryable<Patient> query = _db.Patients.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Mrn))
        {
            var mrn = request.Mrn.Trim();
            query = query.Where(p => p.MRN == mrn);
        }

        if (request.Dob is not null)
        {
            var dayStart = request.Dob.Value.Date;
            var dayEnd = dayStart.AddDays(1);
            query = query.Where(p => p.DOB >= dayStart && p.DOB < dayEnd);
        }

        if (!string.IsNullOrWhiteSpace(request.FirstName))
        {
            var firstName = request.FirstName.Trim() + "%";   // "%" = "anything after", so "Mar" finds "Margaret"
            query = query.Where(p => EF.Functions.Like(p.FirstName, firstName));
        }

        if (!string.IsNullOrWhiteSpace(request.LastName))
        {
            var lastName = request.LastName.Trim() + "%";
            query = query.Where(p => EF.Functions.Like(p.LastName, lastName));
        }

        var patients = await query
            .OrderBy(p => p.LastName)
            .ThenBy(p => p.FirstName)
            .Take(50)                                       // Never return thousands of rows at once
            .ToListAsync();

        return patients.Select(ToDto).ToList();
    }

    public Task<bool> PatientExistsAsync(int patientId)
    {
        return _db.Patients.AnyAsync(p => p.PatientId == patientId);
    }

    public async Task<PatientDto?> GetPatientAsync(int patientId)
    {
        var patient = await _db.Patients.AsNoTracking()
            .FirstOrDefaultAsync(p => p.PatientId == patientId);

        return patient is null ? null : ToDto(patient);
    }

    public async Task<List<AllergyDto>> GetAllergiesAsync(int patientId)
    {
        var allergies = await _db.Allergies.AsNoTracking()
            .Where(a => a.PatientId == patientId)
            .ToListAsync();

        // Active first, then most severe first.
        return allergies
            .OrderBy(a => a.Status == "Active" ? 0 : 1)
            .ThenBy(a => SeverityRank(a.Severity))
            .ThenBy(a => a.AllergyName)
            .Select(a => new AllergyDto(a.AllergyId, a.AllergyName, a.Severity, a.Status, a.RecordedDate))
            .ToList();
    }

    public async Task<List<AlertDto>> GetAlertsAsync(int patientId)
    {
        var alerts = await _db.PatientAlerts.AsNoTracking()
            .Where(a => a.PatientId == patientId && a.Status == "Active")
            .ToListAsync();

        return alerts
            .OrderBy(a => SeverityRank(a.Severity))
            .Select(a => new AlertDto(a.AlertId, a.AlertType, a.Description, a.Severity, a.CreatedDate))
            .ToList();
    }

    public async Task<List<MedicationDto>> GetMedicationsAsync(int patientId, string? status)
    {
        var query = _db.Medications.AsNoTracking().Where(m => m.PatientId == patientId);

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(m => m.Status == status);
        }

        var medications = await query.ToListAsync();

        return medications
            .OrderBy(m => m.Status == "Active" ? 0 : 1)
            .ThenBy(m => m.MedicationName)
            .Select(m => new MedicationDto(m.MedicationId, m.MedicationName, m.Strength, m.DosageInstructions,
                m.Frequency, m.Route, m.StartDate, m.EndDate, m.PrescribingProvider, m.Status))
            .ToList();
    }

    public async Task<List<DocumentDto>> GetDocumentsAsync(int patientId)
    {
        var documents = await _db.PatientDocuments.AsNoTracking()
            .Where(d => d.PatientId == patientId)
            .ToListAsync();

        return documents
            .OrderByDescending(d => d.DocumentDate)
            .Select(d =>
            {
                var extension = Path.GetExtension(d.FilePath).TrimStart('.').ToUpperInvariant();
                var viewMode = extension switch
                {
                    "PDF" => "pdf",
                    "PNG" or "JPG" or "JPEG" or "GIF" => "image",
                    _ => "download"                       // Word files etc. cannot be shown inside a browser
                };
                return new DocumentDto(d.DocumentId, d.DocumentName, d.DocumentType, d.DocumentDate,
                    d.UploadedDate, extension, viewMode);
            })
            .ToList();
    }

    public Task<PatientDocument?> GetDocumentAsync(int documentId)
    {
        return _db.PatientDocuments.AsNoTracking().FirstOrDefaultAsync(d => d.DocumentId == documentId);
    }

    public async Task<PatientHeaderViewModel?> GetHeaderAsync(int patientId, string activePage)
    {
        var patient = await GetPatientAsync(patientId);
        if (patient is null)
        {
            return null;
        }

        var allergies = await GetAllergiesAsync(patientId);
        var alerts = await GetAlertsAsync(patientId);
        return new PatientHeaderViewModel(patient, allergies, alerts, activePage);
    }

    // ---- small helpers ----

    private static PatientDto ToDto(Patient p) => new(
        p.PatientId, p.MRN, p.FirstName, p.LastName, p.DOB, CalculateAge(p.DOB), p.Gender,
        p.PhoneNumber, p.AddressLine1, p.City, p.State, p.ZipCode);

    public static int CalculateAge(DateTime dob)
    {
        var today = DateTime.Today;
        var age = today.Year - dob.Year;
        if (dob.Date > today.AddYears(-age))
        {
            age--;  // Birthday has not happened yet this year
        }
        return age;
    }

    private static int SeverityRank(string severity) => severity switch
    {
        "Severe" or "High" => 0,
        "Moderate" or "Medium" => 1,
        _ => 2
    };
}