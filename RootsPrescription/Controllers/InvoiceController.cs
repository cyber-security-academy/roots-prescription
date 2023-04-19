using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using RootsPrescription.Database;
using RootsPrescription.FileStorage;
using RootsPrescription.Models;
using System.Security.Claims;

namespace RootsPrescription.Controllers;


[Route("[controller]/[action]")]
[ApiController]
public class InvoiceController : ControllerBase
{
    private readonly ILogger<InvoiceController> _logger;
    private readonly IFileStorageService _filestorage;
    private readonly IDatabaseService _dbservice;

    public InvoiceController(ILogger<InvoiceController> logger, IFileStorageService filestorage, IDatabaseService dbservice)
    {
        _logger = logger;
        _filestorage = filestorage;
        _dbservice = dbservice;
    }

    [Authorize]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMyInvoices()
    {
        string authusername = User.FindFirstValue(ClaimTypes.NameIdentifier);
        UserDTO authuser = _dbservice.GetUserByUsername(authusername);
        InvoiceDTO[] invoices = _dbservice.GetUserInvoices(authuser.Id);

        if (invoices == null)
        {
            return NotFound();
        }
        else
        {
            _logger.LogInformation("User {Username} retrieved {NoOfDocs} invoices", authuser.UserName, invoices.Length);
            return Ok(invoices);
        }
    }
    
    [Authorize]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetInvoicePDF(string filename)
    {
        string authusername = User.FindFirstValue(ClaimTypes.NameIdentifier);
        UserDTO authuser = _dbservice.GetUserByUsername(authusername);
    
        InvoiceDTO invoice = _dbservice.GetInvoice(filename);
        if (invoice == null || authuser == null || invoice.OwnerId != authuser.Id)
        {
            _logger.LogWarning("Unauthorized attempt. User '{User}' tried to fetch invoice '{invoice}'", authuser?.Id, invoice?.Id);  // Din loggmelding
            return Unauthorized();  // Returner med statuskode 401
        }
    
        FileStream stream = _filestorage.GetFile(filename);
        if (stream == null)  // file does not exist
        {
            return NotFound();
        }
        else  // file exists
        {
            string attachmentname = Path.GetFileName(stream.Name);
            _logger.LogInformation("Invoice '{Attachment}' downloaded by user {User}", attachmentname, authuser.Id);

            // Respond to client
            Response.Headers.Add("Content-Disposition", $"attachment; filename=\"{attachmentname}\"");
            Response.Headers.Add("X-Content-Type-Options", "nosniff");
            return new FileStreamResult(stream, "application/pdf");
        }
    }
}
