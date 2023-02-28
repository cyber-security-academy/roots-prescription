using Microsoft.AspNetCore.Mvc;
using RootsPrescriptionWin.FileStorage;

namespace RootsPrescriptionWin.Controllers;


[Route("api/[controller]/[action]")]
[ApiController]
public class InvoiceController : ControllerBase
{
    private readonly ILogger<PrescriptionController> _logger;
    private readonly IFileStorageService _filestorage;

    public InvoiceController(ILogger<PrescriptionController> logger, IFileStorageService filestorage)
    {
        _logger = logger;
        _filestorage = filestorage;
    }


    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetInvoice(string filename)
    {
        _logger.LogInformation("Request: " + filename);
        FileStream stream = _filestorage.GetFile(filename);
        if (stream == null)
        {
            return NotFound();
        }
        else
        {
            string attachmentname = Path.GetFileName(stream.Name);

            Response.Headers.Add("Content-Disposition", $"inline; filename=\"{attachmentname}\"");
            Response.Headers.Add("X-Content-Type-Options", "nosniff");
            return new FileStreamResult(stream, "application/pdf");
        }

    }
}
