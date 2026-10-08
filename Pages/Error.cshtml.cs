using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicalView.Pages;

// Shown when the app hits an unexpected error (outside Development mode).
// [AllowAnonymous] so even a logged-out user sees a proper error page.
[AllowAnonymous]
[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
[IgnoreAntiforgeryToken]
public class ErrorModel : PageModel
{
    private readonly ILogger<ErrorModel> _logger;

    public ErrorModel(ILogger<ErrorModel> logger)
    {
        _logger = logger;
    }

    // A reference number the user can give to support. The same number is in the log.
    public string? RequestId { get; set; }

    public void OnGet()
    {
        RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;

        var error = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
        if (error is not null)
        {
            _logger.LogError(error.Error, "Unhandled error on {Path}. Reference {RequestId}", error.Path, RequestId);
        }
    }
}