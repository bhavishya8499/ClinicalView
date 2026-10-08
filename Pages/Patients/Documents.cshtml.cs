using ClinicalView.Dtos;
using ClinicalView.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicalView.Pages.Patients;

public class DocumentsModel : PageModel
{
    private readonly IPatientService _patients;

    public DocumentsModel(IPatientService patients)
    {
        _patients = patients;
    }

    public PatientHeaderViewModel Header { get; private set; } = default!;
    public List<DocumentDto> Documents { get; private set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var header = await _patients.GetHeaderAsync(id, "documents");
        if (header is null)
        {
            return NotFound();
        }

        Header = header;
        Documents = await _patients.GetDocumentsAsync(id);
        return Page();
    }
}