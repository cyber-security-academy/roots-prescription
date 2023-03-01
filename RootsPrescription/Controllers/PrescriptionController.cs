using Microsoft.AspNetCore.Mvc;
using RootsPrescription.FileStorage;

namespace RootsPrescription.Controllers;


[Route("api/[controller]/[action]")]
[ApiController]
public class PrescriptionController : ControllerBase
{
    private readonly ILogger<PrescriptionController> _logger;
    private readonly IFileStorageService _filestorage;

    public PrescriptionController(ILogger<PrescriptionController> logger, IFileStorageService filestorage)
    {
        _logger = logger;
        _filestorage = filestorage;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<string> Ping()
    {
        const string message = "--CHANGE ME--";

        //_logger.Information(helloWorld);
        return message;
    }


    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDoc(int id)
    {
        _logger.LogInformation("Request Prescription: " + id);
        // ToDo: Check that id.owner == u.id
        FileStream stream = _filestorage.GetFile(id);
        if (stream == null)
        {
            return NotFound();
        }
        else
        {
            string attachmentname = Path.GetFileName(stream.Name);
            attachmentname = attachmentname.Replace(".pdf", $"-{id}.pdf");

            Response.Headers.Add("Content-Disposition", $"inline; filename=\"{attachmentname}\"");
            Response.Headers.Add("X-Content-Type-Options", "nosniff");
            return new FileStreamResult(stream, "application/pdf");
        }
    }
}
