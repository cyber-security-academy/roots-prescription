using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using RootsPrescription.Database;
using RootsPrescription.FileStorage;
using RootsPrescription.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using SimpleHashing.Net;
using System.Text.RegularExpressions;

namespace RootsPrescription.Controllers;



[Route("[controller]/[action]")]
[ApiController]
public class SystemController : ControllerBase
{
    private readonly ILogger<SystemController> _logger;

    public SystemController(ILogger<SystemController> logger)
    {
        _logger = logger;
    }

    [AllowAnonymous]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<string> Ping()
    {
        const string message = "--CHANGE ME--";
        _logger.LogInformation(message);

        return message;
    }

    [AllowAnonymous]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<string> Mirror()
    {
        string message = "";

        List<string> __logheaders = new List<string> { "client-ip", "host", "host", "x-client-*", "x-forwarded-for", "x-vb*" };

        List<string> keys = (List<string>)Request.Headers.Keys;
        keys.Sort();
        foreach (string key in keys)
        {

            if (__logheaders.FindAll(x => Regex.IsMatch(key.ToLower(), x)).Count > 0)
            {
                var values = Request.Headers[key];
                message += $"{key}: {String.Join(" ", values)}\n";
            }
        }


        _logger.LogInformation(message);

        return message;
    }


}
