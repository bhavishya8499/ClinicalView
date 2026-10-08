namespace ClinicalView.Models;

public class Medication
{
    public int MedicationId { get; set; }
    public int PatientId { get; set; }
    public string MedicationName { get; set; } = "";
    public string? Strength { get; set; }              // e.g. "10 mg"
    public string? DosageInstructions { get; set; }    // e.g. "Take 1 tablet by mouth daily"
    public string? Frequency { get; set; }             // e.g. "Daily", "BID"
    public string? Route { get; set; }                 // e.g. "PO" (by mouth)
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }             // Empty while the medication is still active
    public string? PrescribingProvider { get; set; }
    public string Status { get; set; } = "Active";     // Active / Discontinued

    public Patient? Patient { get; set; }
}