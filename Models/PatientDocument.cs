namespace ClinicalView.Models;

// Named PatientDocument (not Document) because the PDF library also has a class called Document.
// It is still stored in a table called "Document" (see ApplicationDbContext).
public class PatientDocument
{
    public int DocumentId { get; set; }
    public int PatientId { get; set; }
    public string DocumentName { get; set; } = "";
    public string DocumentType { get; set; } = "";     // e.g. "Lab Result", "Imaging", "Referral"
    public DateTime DocumentDate { get; set; }
    public string FilePath { get; set; } = "";         // File name inside the App_Data/Documents folder
    public DateTime UploadedDate { get; set; }

    public Patient? Patient { get; set; }
}