using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using RootsPrescription.Database;
using RootsPrescription.FileStorage;
using RootsPrescription.Models;
using System.Security.Claims;

namespace RootsPrescription.Controllers;


[Authorize]
[Route("api/[controller]/[action]")]
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
            _logger.LogInformation($"User {authusername} retrieved {prescriptions.Length} prescriptions");
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
        if (prescription == null || prescription.OwnerId != authuser.Id) return Unauthorized();

        FileStream stream = _filestorage.GetFile(prescription.Filename);

        // Respond
        if (stream == null)
        {
            _logger.LogWarning($"The prescription #{id} ({prescription.Filename}) was not found in the file archive");
            return NotFound();
        }
        else
        {
            string attachmentname = Path.GetFileName(stream.Name);
            _logger.LogInformation($"Downloaded {attachmentname}");
            Response.Headers.Add("Content-Disposition", $"inline; filename=\"{attachmentname}\"");
            Response.Headers.Add("X-Content-Type-Options", "nosniff");
            return new FileStreamResult(stream, "application/pdf");
        }
    }
}
