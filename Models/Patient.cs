namespace ClinicalView.Models;

// One row in the Patient table. Each property becomes a column.
public class Patient
{
    public int PatientId { get; set; }                 // Primary key (unique number per patient)
    public string MRN { get; set; } = "";              // Medical Record Number
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public DateTime DOB { get; set; }                  // Date of birth
    public string Gender { get; set; } = "";
    public string? PhoneNumber { get; set; }           // "?" means this value is allowed to be empty (NULL)
    public string? AddressLine1 { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? ZipCode { get; set; }

    // Navigation properties: one patient has many allergies, medications, alerts and documents.
    public List<Allergy> Allergies { get; set; } = new();
    public List<Medication> Medications { get; set; } = new();
    public List<PatientAlert> Alerts { get; set; } = new();
    public List<PatientDocument> Documents { get; set; } = new();
}