using System.ComponentModel.DataAnnotations;

namespace ClinicalView.Dtos;

// DTO = Data Transfer Object: the exact shape of data we send to the browser or API caller.
// We never send database classes directly; DTOs let us choose which fields leave the server.

public record PatientDto(
    int PatientId,
    string MRN,
    string FirstName,
    string LastName,
    DateTime DOB,
    int Age,
    string Gender,
    string? PhoneNumber,
    string? AddressLine1,
    string? City,
    string? State,
    string? ZipCode);

public record AllergyDto(
    int AllergyId,
    string AllergyName,
    string Severity,
    string Status,
    DateTime RecordedDate);

public record AlertDto(
    int AlertId,
    string AlertType,
    string Description,
    string Severity,
    DateTime CreatedDate);

public record MedicationDto(
    int MedicationId,
    string MedicationName,
    string? Strength,
    string? DosageInstructions,
    string? Frequency,
    string? Route,
    DateTime StartDate,
    DateTime? EndDate,
    string? PrescribingProvider,
    string Status);

public record DocumentDto(
    int DocumentId,
    string DocumentName,
    string DocumentType,
    DateTime DocumentDate,
    DateTime UploadedDate,
    string FileExtension,   // "PDF", "PNG", "DOCX" ...
    string ViewMode)        // "pdf" or "image" = show in the browser, "download" = download the file
{
    // URLs of the API endpoint that returns the file.
    public string FileUrl => $"/api/documents/{DocumentId}/file";
    public string DownloadUrl => $"/api/documents/{DocumentId}/file?download=true";
}

// Everything the patient banner and the left menu need, shared by the Chart and Documents pages.
public record PatientHeaderViewModel(
    PatientDto Patient,
    List<AllergyDto> Allergies,
    List<AlertDto> Alerts,
    string ActivePage);     // "chart" or "documents"

// The search form / search API input, with validation rules.
public class PatientSearchRequest : IValidatableObject
{
    [StringLength(20, ErrorMessage = "MRN can be at most 20 characters.")]
    [RegularExpression(@"^[A-Za-z0-9-]+$", ErrorMessage = "MRN can contain only letters, numbers and dashes.")]
    public string? Mrn { get; set; }

    public DateTime? Dob { get; set; }

    [StringLength(50, ErrorMessage = "First name can be at most 50 characters.")]
    [RegularExpression(@"^[A-Za-z' -]+$", ErrorMessage = "First name can contain only letters, spaces, hyphens and apostrophes.")]
    public string? FirstName { get; set; }

    [StringLength(50, ErrorMessage = "Last name can be at most 50 characters.")]
    [RegularExpression(@"^[A-Za-z' -]+$", ErrorMessage = "Last name can contain only letters, spaces, hyphens and apostrophes.")]
    public string? LastName { get; set; }

    // Extra rules that involve more than one field.
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(Mrn) && Dob is null &&
            string.IsNullOrWhiteSpace(FirstName) && string.IsNullOrWhiteSpace(LastName))
        {
            yield return new ValidationResult(
                "Enter at least one search value: MRN, date of birth, first name or last name.");
        }

        if (Dob is not null && Dob.Value.Date > DateTime.Today)
        {
            yield return new ValidationResult("Date of birth cannot be in the future.", new[] { nameof(Dob) });
        }

        if (Dob is not null && Dob.Value.Year < 1900)
        {
            yield return new ValidationResult("Date of birth must be after 1900.", new[] { nameof(Dob) });
        }
    }
}