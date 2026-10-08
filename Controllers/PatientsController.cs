using ClinicalView.Dtos;
using ClinicalView.Services;
using Microsoft.AspNetCore.Mvc;

namespace ClinicalView.Controllers;

// REST API for patient data. Every method returns JSON (or a PDF file).
// [ApiController] turns on automatic validation: bad input gets a 400 response
// before our code even runs.
[ApiController]
[Route("api/patients")]
public class PatientsController : ControllerBase
{
    private readonly IPatientService _patients;
    private readonly PdfReportService _pdf;
    private readonly ILogger<PatientsController> _logger;

    public PatientsController(IPatientService patients, PdfReportService pdf, ILogger<PatientsController> logger)
    {
        _patients = patients;
        _pdf = pdf;
        _logger = logger;
    }

    // GET /api/patients/search?mrn=&dob=&firstName=&lastName=
    [HttpGet("search")]
    public async Task<ActionResult<List<PatientDto>>> Search([FromQuery] PatientSearchRequest request)
    {
        var results = await _patients.SearchAsync(request);
        _logger.LogInformation("User {User} searched patients, {Count} result(s)", User.Identity?.Name, results.Count);
        return Ok(results);
    }

    // GET /api/patients/5/demographics
    [HttpGet("{id:int}/demographics")]
    public async Task<ActionResult<PatientDto>> GetDemographics(int id)
    {
        var patient = await _patients.GetPatientAsync(id);
        if (patient is null)
        {
            return PatientNotFound(id);
        }

        return Ok(patient);
    }

    // GET /api/patients/5/allergies
    [HttpGet("{id:int}/allergies")]
    public async Task<ActionResult<List<AllergyDto>>> GetAllergies(int id)
    {
        if (!await _patients.PatientExistsAsync(id))
        {
            return PatientNotFound(id);
        }

        return Ok(await _patients.GetAllergiesAsync(id));
    }

    // GET /api/patients/5/alerts
    [HttpGet("{id:int}/alerts")]
    public async Task<ActionResult<List<AlertDto>>> GetAlerts(int id)
    {
        if (!await _patients.PatientExistsAsync(id))
        {
            return PatientNotFound(id);
        }

        return Ok(await _patients.GetAlertsAsync(id));
    }

    // GET /api/patients/5/medications            -> all medications
    // GET /api/patients/5/medications?status=Active
    [HttpGet("{id:int}/medications")]
    public async Task<ActionResult<List<MedicationDto>>> GetMedications(int id, [FromQuery] string? status)
    {
        if (status is not null && status != "Active" && status != "Discontinued")
        {
            return Problem(statusCode: StatusCodes.Status400BadRequest,
                title: "Invalid status",
                detail: "Status must be Active or Discontinued. Leave it out to get all medications.");
        }

        if (!await _patients.PatientExistsAsync(id))
        {
            return PatientNotFound(id);
        }

        return Ok(await _patients.GetMedicationsAsync(id, status));
    }

    // GET /api/patients/5/documents
    [HttpGet("{id:int}/documents")]
    public async Task<ActionResult<List<DocumentDto>>> GetDocuments(int id)
    {
        if (!await _patients.PatientExistsAsync(id))
        {
            return PatientNotFound(id);
        }

        return Ok(await _patients.GetDocumentsAsync(id));
    }

    // GET /api/patients/5/summary-pdf  -> downloads a PDF file
    [HttpGet("{id:int}/summary-pdf")]
    public async Task<IActionResult> DownloadSummaryPdf(int id)
    {
        var patient = await _patients.GetPatientAsync(id);
        if (patient is null)
        {
            return PatientNotFound(id);
        }

        var allergies = await _patients.GetAllergiesAsync(id);
        var alerts = await _patients.GetAlertsAsync(id);
        var medications = await _patients.GetMedicationsAsync(id, "Active");
        var printedBy = User.Identity?.Name ?? "unknown user";

        var pdfBytes = _pdf.CreatePatientSummary(patient, allergies, alerts, medications, printedBy);

        _logger.LogInformation("User {User} printed the summary PDF for patient {PatientId}", printedBy, id);

        var fileName = $"{patient.MRN}_{patient.LastName}_Summary_{DateTime.Now:yyyyMMdd}.pdf";
        return File(pdfBytes, "application/pdf", fileName);
    }

    // Same "not found" answer everywhere, in the standard ProblemDetails JSON format.
    private ObjectResult PatientNotFound(int id)
    {
        return Problem(statusCode: StatusCodes.Status404NotFound,
            title: "Patient not found",
            detail: $"There is no patient with id {id}.");
    }
}