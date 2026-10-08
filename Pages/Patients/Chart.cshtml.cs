using ClinicalView.Dtos;
using ClinicalView.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicalView.Pages.Patients;

// Loads everything the chart page shows: banner, demographics, allergies, alerts, medications.
public class ChartModel : PageModel
{
    private readonly IPatientService _patients;
    private readonly ILogger<ChartModel> _logger;

    public ChartModel(IPatientService patients, ILogger<ChartModel> logger)
    {
        _patients = patients;
        _logger = logger;
    }

    public PatientHeaderViewModel Header { get; private set; } = default!;
    public List<MedicationDto> Medications { get; private set; } = new();

    // Which medication tab is selected. Comes from the URL: ?medStatus=Discontinued
    [BindProperty(SupportsGet = true)]
    public string MedStatus { get; set; } = "Active";

    // "id" comes from the URL: /Patients/Chart/5
    public async Task<IActionResult> OnGetAsync(int id)
    {
        var header = await _patients.GetHeaderAsync(id, "chart");
        if (header is null)
        {
            return NotFound();   // shows our friendly 404 page
        }
        Header = header;

        if (MedStatus != "Active" && MedStatus != "Discontinued" && MedStatus != "All")
        {
            MedStatus = "Active";
        }
        Medications = await _patients.GetMedicationsAsync(id, MedStatus == "All" ? null : MedStatus);

        _logger.LogInformation("User {User} opened the chart for patient {PatientId}", User.Identity?.Name, id);
        return Page();
    }
}