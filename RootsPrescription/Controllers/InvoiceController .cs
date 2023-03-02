using Microsoft.AspNetCore.Mvc;
using RootsPrescription.Database;
using RootsPrescription.FileStorage;
using RootsPrescription.Models;

namespace RootsPrescription.Controllers;


[Route("api/[controller]/[action]")]
[ApiController]
public class InvoiceController : ControllerBase
{
    private readonly int DBG_LoggedinUserId = 208;  // TODO: Remove me, when Authentication is in place

    private readonly ILogger<PrescriptionController> _logger;
    private readonly IFileStorageService _filestorage;
    private readonly IDatabaseService _dbservice;

    public InvoiceController(ILogger<PrescriptionController> logger, IFileStorageService filestorage, IDatabaseService dbservice)
    {
        _logger = logger;
        _filestorage = filestorage;
        _dbservice = dbservice;
    }



    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMyInvoices()
    {
        UserDTO authuser = _dbservice.GetUserById(DBG_LoggedinUserId);
        InvoiceDTO[] invoices = _dbservice.GetUserInvoices(DBG_LoggedinUserId);

        if (invoices == null)
        {
            return NotFound();
        }
        else
        {
            _logger.LogInformation($"User {authuser.NationalIdNumber} retrieved {invoices.Length} invoices");
            return Ok(invoices);
        }
    }



    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetInvoice(string filename)
    {
        UserDTO authuser = _dbservice.GetUserById(DBG_LoggedinUserId);
        FileStream stream = _filestorage.GetFile(filename);
        if (stream == null)
        {
            return NotFound();
        }
        else
        {
            string attachmentname = Path.GetFileName(stream.Name);

            _logger.LogInformation($"User {authuser.NationalIdNumber} requested invoice: " + attachmentname);

            Response.Headers.Add("Content-Disposition", $"inline; filename=\"{attachmentname}\"");
            Response.Headers.Add("X-Content-Type-Options", "nosniff");
            return new FileStreamResult(stream, "application/pdf");
        }

    }
}
