using Microsoft.AspNetCore.Mvc;
using RootsPrescriptionWin.FileStorage;

namespace RootsPrescriptionWin.Controllers;


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
    public ActionResult<string> Fetch(string id)
    {
        if (id == null || id == "404")
        {
            return NotFound();
        }
        else
        {
            _logger.LogInformation("Request: " + id);
            string file = _filestorage.GetFile(id);
            return file;
        }

    }

}
