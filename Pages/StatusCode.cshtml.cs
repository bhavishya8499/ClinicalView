using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicalView.Pages;

// Friendly page for "not found" and other status codes.
// Program.cs sends those here with: app.UseStatusCodePagesWithReExecute("/StatusCode/{0}")
[AllowAnonymous]
public class StatusCodeModel : PageModel
{
    public int Code { get; private set; }
    public string Heading { get; private set; } = "";
    public string Message { get; private set; } = "";

    public void OnGet(int code)
    {
        Code = code;
        (Heading, Message) = code switch
        {
            404 => ("Not found", "The patient or page you are looking for doesn't exist. It may have been moved, or the link may be wrong."),
            403 => ("Access denied", "Your account doesn't have permission to view this page."),
            _ => ("Something went wrong", "The request could not be completed. Please try again.")
        };
    }
}