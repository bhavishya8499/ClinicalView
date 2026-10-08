namespace ClinicalView.Models;

// The sample schema in the assessment has no Alert table, but the requirements ask to display alerts,
// so this table is added.
public class PatientAlert
{
    public int AlertId { get; set; }
    public int PatientId { get; set; }
    public string AlertType { get; set; } = "";        // e.g. "Fall Risk", "Isolation", "Code Status"
    public string Description { get; set; } = "";
    public string Severity { get; set; } = "";         // High / Medium / Low
    public string Status { get; set; } = "Active";     // Active / Resolved
    public DateTime CreatedDate { get; set; }

    public Patient? Patient { get; set; }
}