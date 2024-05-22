using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using RootsPrescription.Database;
using RootsPrescription.FileStorage;
using RootsPrescription.Models;
using System.Security.Claims;

namespace RootsPrescription.Controllers;


[Authorize]
[Route("[controller]/[action]")]
[ApiController]
public class PrescriptionController : ControllerBase
{

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
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMyPrescriptions()
    {
        // Find prescriptions for the authenticated `User`
        string authusername = User.FindFirstValue(ClaimTypes.NameIdentifier);
        UserDTO authuser = _dbservice.GetUserByUsername(authusername);
        PrescriptionDTO[] prescriptions = _dbservice.GetUserPrescriptions(authuser.Id);

        // Respond
        if (prescriptions == null)
        {
            return NotFound();
        }
        else
        {
            _logger.LogInformation("User {Username} retrieved {NoOfDocs} prescriptions", authusername, prescriptions.Length);
            return Ok(prescriptions);
        }
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPDF(int id)
    {
        // Look up username for the authenticated `User`
        string authusername = User.FindFirstValue(ClaimTypes.NameIdentifier);
        UserDTO authuser = _dbservice.GetUserByUsername(authusername);

        // Verify that the prescription is owned by the user
        PrescriptionDTO prescription = _dbservice.GetPrescription(id);
        if (prescription == null || prescription.OwnerId != authuser.Id)
        {
            string docExists = (prescription == null) ? "non-exist" : "existing";
            _logger.LogWarning("Unauthorized attempt by user {authUserName} of fetching ({docExists}) prescription id #{id}", authuser?.UserName, docExists, id);
            return Unauthorized();
        }

        // Check if file exists
        FileStream stream = _filestorage.GetFile(prescription.Filename);
        if (stream == null)
        {
            _logger.LogWarning("The prescription #{Id} ({Filename}) was not found in the file archive", id, prescription.Filename);
            return NotFound();
        }
        else  // file exists
        {
            string attachmentname = Path.GetFileName(stream.Name);
            _logger.LogInformation("Downloaded: {Attachment}", attachmentname);

            // Respond to client
            Response.Headers.Add("Content-Disposition", $"attachment; filename=\"{attachmentname}\"");
            Response.Headers.Add("X-Content-Type-Options", "nosniff");
            return new FileStreamResult(stream, "application/pdf");
        }
    }
}
