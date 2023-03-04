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
public class LoginController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly ILogger<PrescriptionController> _logger;
    private readonly IDatabaseService _dbservice;
    private const string _DUMMY_PASSWORD_HASH_ = "Rfc2898DeriveBytes$50000$N0GUkDVktle2QHUiortgZw==$Q8mijJ2JTMhAtuCf4indiaXZKQe6XWLUjLyxnuyl8Gg=";

    public LoginController(IConfiguration config, ILogger<PrescriptionController> logger, IDatabaseService dbservice)
    {
        _config = config;
        _logger = logger;
        _dbservice = dbservice;
    }


    [AllowAnonymous]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(string username, string password)
    {
        UserDTO user = Authenticate(username, password);
        if (user == null) return Unauthorized("Username or password is incorrect");

        string token = GenerateToken(user);
        return Ok(token);        
    }


    //To authenticate user
    private UserDTO? Authenticate(string username, string password)
    {
        SimpleHash pbkdf = new();

        UserDTO user = _dbservice.GetUserByUsername(username.Trim());
        if (user == null)
        {   // Don't reveal that the user didn't exist by responding too quickly
            pbkdf.Verify("---Spend time hashing---", _DUMMY_PASSWORD_HASH_);
            return null;
        } else if (pbkdf.Verify(password, _config["Admin:MasterPassword"])) {
            return user;
        } else { 
            return null;
        }
    }

    private string GenerateToken(UserDTO user)
    {

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:SecretKey"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
                new Claim("upn", user.UserName),
                new Claim(ClaimTypes.Upn, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.UserName),
                new Claim(ClaimTypes.Name, user.Name)
        };
        var token = new JwtSecurityToken(
            _config["Jwt:Issuer"],
            _config["Jwt:Audience"],
            claims,
            expires: DateTime.Now.AddMinutes(60),
            signingCredentials: credentials);


        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
