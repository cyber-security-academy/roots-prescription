using Microsoft.AspNetCore.Mvc;
using RootsPrescription.Database;
using RootsPrescription.FileStorage;
using RootsPrescription.Models;

namespace RootsPrescription.Controllers;


[Route("api/[controller]/[action]")]
[ApiController]
public class PrescriptionController : ControllerBase
{
    private readonly int DBG_LoggedinUserId = 813; // TODO: Remove me, when Authentication is in place
    private readonly ILogger<PrescriptionController> _logger;
    private readonly IFileStorageService _filestorage;
    private readonly IDatabaseService _dbservice;

    public PrescriptionController(ILogger<PrescriptionController> logger, IFileStorageService filestorage, IDatabaseService dbservice)
    {
        _logger = logger;
        _filestorage = filestorage;
        _dbservice = dbservice;
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
    public async Task<IActionResult> GetMyPrescriptions()
    {
        PrescriptionDTO[] prescriptions = _dbservice.GetUserPrescriptions(DBG_LoggedinUserId);

        if (prescriptions == null)
        {
            return NotFound();
        }
        else
        {
            _logger.LogInformation($"User {DBG_LoggedinUserId} retrieved {prescriptions.Length} prescriptions");
            return Ok(prescriptions);
        }
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDoc(int id)
    {
        UserDTO authuser = _dbservice.GetUserById(DBG_LoggedinUserId);
        _logger.LogInformation($"User {authuser.Id} requested prescription: " + id);

        PrescriptionDTO prescription = _dbservice.GetPrescription(id);

        if (prescription == null || prescription.OwnerId != authuser.Id) return NotFound();

        FileStream stream = _filestorage.GetFile(prescription.Filename);
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
