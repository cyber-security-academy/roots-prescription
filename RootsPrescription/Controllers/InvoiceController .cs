using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using RootsPrescription.Database;
using RootsPrescription.FileStorage;
using RootsPrescription.Models;
using System.Security.Claims;
using System.Diagnostics;
using Splunk.Logging;

namespace RootsPrescription.Controllers;


[Route("api/[controller]/[action]")]
[ApiController]
public class InvoiceController : ControllerBase
{
    private readonly ILogger<InvoiceController> _logger;
    private readonly IFileStorageService _filestorage;
    private readonly IDatabaseService _dbservice;
    private static TraceSource trace;

    public InvoiceController(ILogger<InvoiceController> logger, IFileStorageService filestorage, IDatabaseService dbservice)
    {
        _logger = logger;
        _filestorage = filestorage;
        _dbservice = dbservice;

        trace = new TraceSource("RootsPrescription");
        trace.Switch.Level = SourceLevels.All;
        trace.Listeners.Clear();
        trace.Listeners.Add(new HttpEventCollectorTraceListener(
            uri: new Uri("https://log.splunk.csa.datasnok.no"),  // must be ENV var
            token: "ab98f781-16d4-43f4-a098-611cfb20db3f"));  // must be ENV var
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
			string logMsg = $"User {authuser.NationalIdNumber} {authuser.UserName} retrieved {invoices.Length} invoices";
            _logger.LogInformation(logMsg);
			trace.TraceEvent(TraceEventType.Information, 0, logMsg);

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
            string attachmentname = Path.GetFileName(stream.Name);
			string logMsg = $"Downloaded: {attachmentname}";
            _logger.LogInformation(logMsg);
			trace.TraceEvent(TraceEventType.Information, 1, logMsg);

            // Respond to client
            Response.Headers.Add("Content-Disposition", $"inline; filename=\"{attachmentname}\"");
            Response.Headers.Add("X-Content-Type-Options", "nosniff");
            return new FileStreamResult(stream, "application/pdf");
        }

    }
}
