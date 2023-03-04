﻿using Microsoft.AspNetCore.Mvc;
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


    [Authorize]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMyInvoices()
    {
        string authusername = User.FindFirstValue(ClaimTypes.NameIdentifier);
        UserDTO authuser = _dbservice.GetUserByUsername(authusername);
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
            _logger.LogInformation($"Download invoice: " + attachmentname);

            // Respond to client
            Response.Headers.Add("Content-Disposition", $"inline; filename=\"{attachmentname}\"");
            Response.Headers.Add("X-Content-Type-Options", "nosniff");
            return new FileStreamResult(stream, "application/pdf");
        }

    }
}
