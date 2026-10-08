using ClinicalView.Dtos;
using ClinicalView.Models;

namespace ClinicalView.Services;

// The interface lists WHAT the service can do. Pages and APIs depend on this,
// not on the database directly.
public interface IPatientService
{
    Task<List<PatientDto>> SearchAsync(PatientSearchRequest request);
    Task<bool> PatientExistsAsync(int patientId);
    Task<PatientDto?> GetPatientAsync(int patientId);
    Task<List<AllergyDto>> GetAllergiesAsync(int patientId);
    Task<List<AlertDto>> GetAlertsAsync(int patientId);
    Task<List<MedicationDto>> GetMedicationsAsync(int patientId, string? status);
    Task<List<DocumentDto>> GetDocumentsAsync(int patientId);
    Task<PatientDocument?> GetDocumentAsync(int documentId);
    Task<PatientHeaderViewModel?> GetHeaderAsync(int patientId, string activePage);
}