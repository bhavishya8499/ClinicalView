using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicalView.Pages;

// The search page has no server-side work to do when it loads.
// The search itself runs in the browser (wwwroot/js/search.js), which calls /api/patients/search.
public class IndexModel : PageModel
{
    public void OnGet()
    {
    }
}