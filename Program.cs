using ClinicalView.Data;
using ClinicalView.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------------------------
// PART 1: register services (the "parts" the app can ask for)
// ---------------------------------------------------------------

// Database: EF Core talks to the SQLite file named in appsettings.json.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Login system (ASP.NET Core Identity): user accounts are stored in our database.
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;   // no email server in this demo
    options.Password.RequiredLength = 8;
})
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Google sign-in. Only switched on when the Client ID and Secret exist in User Secrets,
// so the app still runs (with the local demo account) on a computer that doesn't have them.
var googleClientId = builder.Configuration["Authentication:Google:ClientId"];
var googleClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
if (!string.IsNullOrEmpty(googleClientId) && !string.IsNullOrEmpty(googleClientSecret))
{
    builder.Services.AddAuthentication().AddGoogle(options =>
    {
        options.ClientId = googleClientId;
        options.ClientSecret = googleClientSecret;
    });
}

// Login cookie: sign out after 20 idle minutes, like most clinical systems.
builder.Services.ConfigureApplicationCookie(options =>
{
    options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
    options.SlidingExpiration = true;

    // Browsers get sent to the login page, but API callers get a plain 401 answer.
    options.Events.OnRedirectToLogin = context =>
    {
        if (context.Request.Path.StartsWithSegments("/api"))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        }
        else
        {
            context.Response.Redirect(context.RedirectUri);
        }
        return Task.CompletedTask;
    };
});

// Every page and API needs a logged-in user, unless it is marked [AllowAnonymous].
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

// Our own classes. "Scoped" = one new copy per web request.
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddSingleton<PdfReportService>();
QuestPDF.Settings.License = LicenseType.Community;     // free license for individuals and small companies

builder.Services.AddRazorPages();
builder.Services.AddControllers();
builder.Services.AddProblemDetails();

var app = builder.Build();

// ---------------------------------------------------------------
// PART 2: create the database and the demo data on startup
// ---------------------------------------------------------------
using (var scope = app.Services.CreateScope())
{
    await SeedData.InitializeAsync(scope.ServiceProvider);
}

// ---------------------------------------------------------------
// PART 3: the request pipeline (every request passes through these in order)
// ---------------------------------------------------------------
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");     // friendly error page instead of a crash screen
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/StatusCode/{0}");   // friendly 404 page

app.UseHttpsRedirection();
app.UseStaticFiles();          // CSS, JavaScript, images from wwwroot

app.UseRouting();

app.UseAuthentication();       // who are you?
app.UseAuthorization();        // are you allowed in?

app.MapRazorPages();           // the screens
app.MapControllers();          // the /api endpoints

app.Run();