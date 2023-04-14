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


    
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetInvoicePDF(string filename)
    {
        /* Oppgave 5.1: Logg hvilken bruker som gjør kallet
           Kommenter ut linjene under og endre loggmeldingen på linje 72 
           til noe dere tenker er riktig */
        // string authusername = User.FindFirstValue(ClaimTypes.NameIdentifier);
        // UserDTO authuser = _dbservice.GetUserByUsername(authusername);

        /* Oppgave 5.2: Logg forsøk på kall av brukere som ikke er autorisert
           Kommenter ut linjene under og skriv den meldingen dere tenker er riktig */
        // InvoiceDTO invoice = _dbservice.GetInvoice(id)
        // if (invoice == null || invoice.OwnerId != authuser.Id)
        // {
        //     _logger.LogWarning("");
        //     return Unauthorized();
        // }


        // Check if file exists
        FileStream stream = _filestorage.GetFile(filename);
        if (stream == null)
        {
            /* Oppgave 5.3: Logg når noen prøver å hente en fil som ikke finnes
               Legg til en loggmeldingen dere tenker er riktig under  */
            // Bytt meg ut med loggmelding!
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
