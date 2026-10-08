using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicalView.Areas.Identity.Pages.Account;

// Our own version of the login page. Because it sits at Areas/Identity/Pages/Account/Login,
// it replaces the plain built-in Identity login page. Register, Logout and the Google
// callback still use the built-in pages.
[AllowAnonymous]
public class LoginModel : PageModel
{
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly ILogger<LoginModel> _logger;

    public LoginModel(SignInManager<IdentityUser> signInManager, ILogger<LoginModel> logger)
    {
        _signInManager = signInManager;
        _logger = logger;
    }

    // [BindProperty] fills Input from the posted form fields automatically.
    [BindProperty]
    public InputModel Input { get; set; } = new();

    // Google (and any other external providers that are switched on in Program.cs).
    public List<AuthenticationScheme> ExternalLogins { get; set; } = new();

    // Where to go after logging in (the page the user originally asked for).
    public string ReturnUrl { get; set; } = "/";

    // Lets the built-in Google callback page pass an error message back to us.
    [TempData]
    public string? ErrorMessage { get; set; }

    public class InputModel
    {
        [Required(ErrorMessage = "Enter your email.")]
        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Enter your password.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = "";

        [Display(Name = "Keep me signed in")]
        public bool RememberMe { get; set; }
    }

    public async Task OnGetAsync(string? returnUrl = null)
    {
        if (!string.IsNullOrEmpty(ErrorMessage))
        {
            ModelState.AddModelError(string.Empty, ErrorMessage);
        }

        ReturnUrl = returnUrl ?? Url.Content("~/");

        // Clear any half-finished Google login so the user starts clean.
        await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

        ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        ReturnUrl = returnUrl ?? Url.Content("~/");
        ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

        // Validation attributes on InputModel failed (empty email, bad format...): show the form again.
        if (!ModelState.IsValid)
        {
            return Page();
        }

        // lockoutOnFailure: after 5 wrong passwords the account is locked for 5 minutes.
        var result = await _signInManager.PasswordSignInAsync(
            Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: true);

        if (result.Succeeded)
        {
            _logger.LogInformation("User {Email} logged in", Input.Email);
            return LocalRedirect(ReturnUrl);   // LocalRedirect refuses links to other websites
        }

        if (result.IsLockedOut)
        {
            _logger.LogWarning("User {Email} is locked out", Input.Email);
            ModelState.AddModelError(string.Empty, "This account is locked after too many failed attempts. Try again in 5 minutes.");
            return Page();
        }

        // Same message for "no such user" and "wrong password", so nobody can find out which emails exist.
        ModelState.AddModelError(string.Empty, "Incorrect email or password.");
        return Page();
    }
}