using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using RootsPrescription.Database;
using RootsPrescription.FileStorage;
using RootsPrescription.Models;
using System.Security.Claims;

namespace RootsPrescription.Controllers;


[Route("api/[controller]/[action]")]
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
            _logger.LogInformation($"User {authuser.UserName} retrieved {invoices.Length} invoices");
            foreach (string header in Request.Headers.Keys)
            {
                _logger.LogInformation($"Test: Headers: {header}: {Request.Headers[header]}");
            }
            return Ok(invoices);
        }
    }


    
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetInvoicePDF(string filename)
    {
        // Check if Invoice exists
        FileStream stream = _filestorage.GetFile(filename);
        if (stream == null)
        {
            return NotFound();
        }
        else
        {
            foreach (string header in Request.Headers.Keys)
            {
                _logger.LogInformation($"Test: Headers: {header}: {Request.Headers[header]}");
            }

            string attachmentname = Path.GetFileName(stream.Name);
            _logger.LogInformation($"Downloaded: {attachmentname}");

            // Respond to client
            Response.Headers.Add("Content-Disposition", $"attachment; filename=\"{attachmentname}\"");
            Response.Headers.Add("X-Content-Type-Options", "nosniff");
            return new FileStreamResult(stream, "application/pdf");
        }

    }
}
