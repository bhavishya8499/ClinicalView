# ClinicalView – Patient Information Portal

A small clinical web app for searching patients and reviewing their chart: demographics, allergies, alerts, medications and documents, with a one-click PDF summary. Built for the Stanford Health Care Clinical Systems Analyst assessment (R2657612).

All patient data in this project is made up.

## What it does

- **Sign in** with Google, or with a local account (demo account below). Sessions end after 20 idle minutes.
- **Patient search** by MRN, date of birth, first name or last name. Partial names work ("Harr" finds Harrison, Harris, Harrington).
- **Patient chart** with a banner that always shows allergies and alerts, then demographics, allergies, alerts and medications (Active / Discontinued / All).
- **Documents** list for each patient. PDFs and images open in a preview window; Word files download.
- **Print to PDF** on the demographics section creates a summary of demographics, allergies, alerts and active medications.
- **REST API** for search, demographics, allergies, alerts, medications and documents.
- Works on desktop, tablet and phone.

## Tech stack

| Area | Choice |

| Backend | ASP.NET Core 8, C#, Razor Pages + API controllers |
| Database | SQLite through Entity Framework Core 8 (code-first migrations) |
| Login | ASP.NET Core Identity + Google OAuth |
| PDF | QuestPDF (Community license) |
| Frontend | HTML5, CSS3, JavaScript (fetch API), Bootstrap 5 |
| IDE | Visual Studio 2022 |

## Running it

1. Install Visual Studio 2022 (ASP.NET and web development workload) and the .NET 8 SDK.
2. Clone the repo and open `ClinicalView.csproj` in Visual Studio.
3. Press **F5**. On first start the app creates the SQLite database (`app.db`), the tables, the demo user and 12 sample patients.
4. Sign in with the demo account:
   - Email: `demo@clinic.test`
   - Password: `Demo@12345`

### Turning on Google sign-in (optional)

Google sign-in switches on automatically when a Client ID and Secret are present. They are kept in User Secrets, never in the code.

1. In Google Cloud Console, create an OAuth client of type **Web application** with this redirect URI: `https://localhost:<port>/signin-google` (the port is in `Properties/launchSettings.json`).
2. In Visual Studio, right-click the project, choose **Manage User Secrets**, and paste:

```json
{
  "Authentication:Google:ClientId": "your-client-id",
  "Authentication:Google:ClientSecret": "your-client-secret"
}
```

3. Run the app. A "Sign in with Google" button appears on the login page.

## API

All endpoints require a signed-in user. Without one they return `401`. Errors use the standard ProblemDetails JSON format.

| Method | URL | Returns |
|---|---|---|
| GET | `/api/patients/search?mrn=&dob=&firstName=&lastName=` | Matching patients (max 50). At least one field is required. |
| GET | `/api/patients/{id}/demographics` | One patient |
| GET | `/api/patients/{id}/allergies` | Allergies, active and most severe first |
| GET | `/api/patients/{id}/alerts` | Active alerts |
| GET | `/api/patients/{id}/medications?status=Active` | Medications. `status` is optional (Active or Discontinued). |
| GET | `/api/patients/{id}/documents` | Document list |
| GET | `/api/patients/{id}/summary-pdf` | PDF summary file |
| GET | `/api/documents/{id}/file?download=true` | The document file (inline, or as a download) |

## Project layout

```
Controllers/   API endpoints (PatientsController, DocumentsController)
Data/          DbContext, migrations, sample data and startup seeding
Dtos/          The shapes of data sent to the browser, plus search validation rules
Helpers/       Date and name formatting shared by pages and the PDF
Models/        Database tables as C# classes
Services/      PatientService (database queries) and PdfReportService
Pages/         Razor Pages: search, patient chart, documents, error pages
Areas/Identity Custom login page
wwwroot/       CSS and JavaScript
App_Data/      Sample document files (served only through the API)
```

## Design decisions

- **Alert table added.** The sample schema had no table for alerts, but the requirements ask to show them, so I added `Alert` (type, description, severity, status, date).
- **Documents are stored outside `wwwroot`.** Files in `wwwroot` can be opened by anyone who guesses the URL. Here, files are only returned by the API after sign-in, and the API rejects any file path that tries to leave the documents folder.
- **Every page and API needs sign-in by default** (a fallback authorization policy), so a new page can't accidentally be left public.
- **Validation runs twice:** in the browser for quick feedback, and on the server, which is the one that counts.
- **Services between pages and the database.** Pages and APIs call `IPatientService` instead of the database directly, so the same queries are reused and could be unit tested.
- **SQLite** so a reviewer can run the project with no database install.
- **Clinical display conventions:** names as LAST, First; DOB with age; allergies shown in the banner on every chart page; "No Known Allergies" stated explicitly instead of an empty table.
- **Logging** of searches, chart views, document opens and PDF prints, as a starting point for an audit trail.

## What I'd add with more time

- A proper audit log table and screen (who viewed which chart, and when)
- Role-based access (for example, front desk vs. clinician)
- Unit tests for the search service and validation
- FHIR R4 endpoints (Patient, AllergyIntolerance, MedicationStatement)
- Deployment to Azure App Service with Azure SQL
