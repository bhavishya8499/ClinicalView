using System.Globalization;
using ClinicalView.Dtos;

namespace ClinicalView.Helpers;

// Small formatting helpers used by the pages and the PDF, so dates and names
// look the same everywhere in the app.
public static class Display
{
    // 03/14/1958. InvariantCulture keeps the "/" even if the computer is set to another country's date format.
    public static string Date(DateTime? value) =>
        value.HasValue ? value.Value.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture) : "-";

    // 09/22/2026 2:00 PM
    public static string DateAndTime(DateTime? value) =>
        value.HasValue ? value.Value.ToString("MM/dd/yyyy h:mm tt", CultureInfo.InvariantCulture) : "-";

    // Hospitals usually show names as "LAST, First".
    public static string PatientName(PatientDto patient) =>
        $"{patient.LastName.ToUpperInvariant()}, {patient.FirstName}";

    // "MH" for Margaret Harrison
    public static string Initials(PatientDto patient)
    {
        var first = patient.FirstName.Length > 0 ? patient.FirstName[..1] : "";
        var last = patient.LastName.Length > 0 ? patient.LastName[..1] : "";
        return (first + last).ToUpperInvariant();
    }

    // Picks the CSS class (color) for a severity label.
    public static string SeverityPill(string severity) => severity switch
    {
        "Severe" or "High" => "pill pill-red",
        "Moderate" or "Medium" => "pill pill-amber",
        _ => "pill pill-gray"
    };

    public static string StatusPill(string status) => status switch
    {
        "Active" => "pill pill-green",
        _ => "pill pill-gray"
    };

    public static string OrDash(string? value) => string.IsNullOrWhiteSpace(value) ? "-" : value;
}