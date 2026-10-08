using ClinicalView.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace ClinicalView.Controllers;

// Sends a document file to the browser.
// The files live in App_Data/Documents, which is NOT inside wwwroot, so nobody can
// open them by guessing a URL. They only come out through this endpoint, after login.
[ApiController]
[Route("api/documents")]
public class DocumentsController : ControllerBase
{
    private readonly IPatientService _patients;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<DocumentsController> _logger;

    public DocumentsController(IPatientService patients, IWebHostEnvironment environment,
        ILogger<DocumentsController> logger)
    {
        _patients = patients;
        _environment = environment;
        _logger = logger;
    }

    // GET /api/documents/3/file                -> shows the file in the browser (PDF, images)
    // GET /api/documents/3/file?download=true  -> downloads the file
    [HttpGet("{id:int}/file")]
    public async Task<IActionResult> GetFile(int id, [FromQuery] bool download = false)
    {
        var document = await _patients.GetDocumentAsync(id);
        if (document is null)
        {
            return Problem(statusCode: StatusCodes.Status404NotFound,
                title: "Document not found",
                detail: $"There is no document with id {id}.");
        }

        var documentsFolder = Path.GetFullPath(Path.Combine(_environment.ContentRootPath, "App_Data", "Documents"));
        var fullPath = Path.GetFullPath(Path.Combine(documentsFolder, document.FilePath));

        // Safety check: a file path like "..\..\appsettings.json" must never escape the Documents folder.
        if (!fullPath.StartsWith(documentsFolder + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogWarning("Blocked document {DocumentId} with an unsafe file path", id);
            return Problem(statusCode: StatusCodes.Status400BadRequest, title: "Invalid document path");
        }

        if (!System.IO.File.Exists(fullPath))
        {
            _logger.LogWarning("File for document {DocumentId} is missing: {Path}", id, fullPath);
            return Problem(statusCode: StatusCodes.Status404NotFound,
                title: "File missing",
                detail: "The document exists in the database, but its file was not found on the server.");
        }

        // Work out the file type from the extension (.pdf -> application/pdf, .png -> image/png, ...)
        var contentTypes = new FileExtensionContentTypeProvider();
        if (!contentTypes.TryGetContentType(fullPath, out var contentType))
        {
            contentType = "application/octet-stream";
        }

        _logger.LogInformation("User {User} opened document {DocumentId} for patient {PatientId}",
            User.Identity?.Name, id, document.PatientId);

        if (download)
        {
            var downloadName = document.DocumentName + Path.GetExtension(fullPath);
            return PhysicalFile(fullPath, contentType, downloadName);
        }

        return PhysicalFile(fullPath, contentType);
    }
}