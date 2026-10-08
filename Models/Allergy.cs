namespace ClinicalView.Models;

public class Allergy
{
    public int AllergyId { get; set; }
    public int PatientId { get; set; }                 // Foreign key: which patient this allergy belongs to
    public string AllergyName { get; set; } = "";
    public string Severity { get; set; } = "";         // Severe / Moderate / Mild
    public string Status { get; set; } = "Active";     // Active / Inactive
    public DateTime RecordedDate { get; set; }

    public Patient? Patient { get; set; }
}