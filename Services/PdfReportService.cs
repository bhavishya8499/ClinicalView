using ClinicalView.Dtos;
using ClinicalView.Helpers;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace ClinicalView.Services;

// Builds the "Print to PDF" file with the QuestPDF library.
// QuestPDF describes a page as nested boxes: Page -> Header / Content / Footer -> Column -> Item -> Text or Table.
public class PdfReportService
{
    private const string Navy = "#0B4F8A";
    private const string BorderGray = "#D8DEE6";
    private const string LabelGray = "#5B6B7C";
    private const string HeaderFill = "#EEF3F8";

    public byte[] CreatePatientSummary(PatientDto patient, List<AllergyDto> allergies,
        List<AlertDto> alerts, List<MedicationDto> medications, string printedBy)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.Letter);
                page.Margin(36);                                   // 36 points = half an inch
                page.DefaultTextStyle(style => style.FontSize(10));

                // Top of every page
                page.Header().Column(header =>
                {
                    header.Item().Text("Patient Clinical Summary").FontSize(18).Bold().FontColor(Navy);
                    header.Item().Text(
                        $"{Display.PatientName(patient)}   |   MRN {patient.MRN}   |   " +
                        $"DOB {Display.Date(patient.DOB)} ({patient.Age} y)   |   {patient.Gender}");
                    header.Item().PaddingTop(6).LineHorizontal(1).LineColor(BorderGray);
                });

                // Main body: one section under another
                page.Content().PaddingTop(12).Column(body =>
                {
                    body.Spacing(16);

                    body.Item().Column(section =>
                    {
                        section.Item().PaddingBottom(4).Text("Demographics").FontSize(13).Bold().FontColor(Navy);
                        section.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(90);
                                columns.RelativeColumn();
                                columns.ConstantColumn(90);
                                columns.RelativeColumn();
                            });

                            AddField(table, "Name", $"{patient.FirstName} {patient.LastName}");
                            AddField(table, "MRN", patient.MRN);
                            AddField(table, "Date of birth", $"{Display.Date(patient.DOB)} ({patient.Age} y)");
                            AddField(table, "Gender", patient.Gender);
                            AddField(table, "Phone", patient.PhoneNumber);
                            AddField(table, "Address",
                                $"{patient.AddressLine1}, {patient.City}, {patient.State} {patient.ZipCode}");
                        });
                    });

                    body.Item().Column(section =>
                    {
                        section.Item().PaddingBottom(4).Text("Allergies").FontSize(13).Bold().FontColor(Navy);
                        if (allergies.Count == 0)
                        {
                            section.Item().Text("No Known Allergies");
                        }
                        else
                        {
                            section.Item().Element(box => DataTable(box,
                                new[] { "Allergen", "Severity", "Status", "Recorded" },
                                new float[] { 3, 2, 2, 2 },
                                allergies.Select(a => new[]
                                {
                                    a.AllergyName, a.Severity, a.Status, Display.Date(a.RecordedDate)
                                }).ToList()));
                        }
                    });

                    body.Item().Column(section =>
                    {
                        section.Item().PaddingBottom(4).Text("Alerts").FontSize(13).Bold().FontColor(Navy);
                        if (alerts.Count == 0)
                        {
                            section.Item().Text("No active alerts");
                        }
                        else
                        {
                            section.Item().Element(box => DataTable(box,
                                new[] { "Alert", "Severity", "Details" },
                                new float[] { 2, 1, 5 },
                                alerts.Select(a => new[] { a.AlertType, a.Severity, a.Description }).ToList()));
                        }
                    });

                    body.Item().Column(section =>
                    {
                        section.Item().PaddingBottom(4).Text("Active Medications").FontSize(13).Bold().FontColor(Navy);
                        if (medications.Count == 0)
                        {
                            section.Item().Text("No active medications");
                        }
                        else
                        {
                            section.Item().Element(box => DataTable(box,
                                new[] { "Medication", "Instructions", "Route", "Frequency", "Start", "Prescriber" },
                                new float[] { 3, 4, 1.2f, 1.5f, 1.6f, 2 },
                                medications.Select(m => new[]
                                {
                                    $"{m.MedicationName} {m.Strength}",
                                    Display.OrDash(m.DosageInstructions),
                                    Display.OrDash(m.Route),
                                    Display.OrDash(m.Frequency),
                                    Display.Date(m.StartDate),
                                    Display.OrDash(m.PrescribingProvider)
                                }).ToList()));
                        }
                    });
                });

                // Bottom of every page
                page.Footer().Row(row =>
                {
                    row.RelativeItem().Text(
                        $"Printed {Display.DateAndTime(DateTime.Now)} by {printedBy}. " +
                        "CONFIDENTIAL: contains protected health information.")
                        .FontSize(8).FontColor(LabelGray);

                    row.AutoItem().Text(text =>
                    {
                        text.Span("Page ");
                        text.CurrentPageNumber();
                        text.Span(" of ");
                        text.TotalPages();
                    });
                });
            });
        });

        return document.GeneratePdf();
    }

    // One "label: value" pair in the demographics grid.
    private static void AddField(TableDescriptor table, string label, string? value)
    {
        table.Cell().PaddingVertical(3).Text(label).Bold().FontColor(LabelGray);
        table.Cell().PaddingVertical(3).Text(Display.OrDash(value));
    }

    // A simple table with a shaded header row and a line under each row.
    private static void DataTable(IContainer container, string[] headers, float[] columnWidths, List<string[]> rows)
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                foreach (var width in columnWidths)
                {
                    columns.RelativeColumn(width);
                }
            });

            table.Header(header =>
            {
                foreach (var title in headers)
                {
                    header.Cell().Background(HeaderFill).Padding(4).Text(title).Bold();
                }
            });

            foreach (var row in rows)
            {
                foreach (var cell in row)
                {
                    table.Cell().BorderBottom(1).BorderColor(BorderGray).Padding(4).Text(cell);
                }
            }
        });
    }
}