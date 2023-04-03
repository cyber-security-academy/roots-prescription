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

namespace RootsPrescription.Controllers;



[Route("api/[controller]/[action]")]
[ApiController]
public class SystemController : ControllerBase
{
    public SystemController()
    {
    }

    [AllowAnonymous]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<string> Ping()
    {
        const string message = "Bip bop";

        return message;
    }


}
