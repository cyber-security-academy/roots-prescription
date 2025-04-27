using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using RootsPrescription.Database;
using RootsPrescription.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using SimpleHashing.Net;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace RootsPrescription.Controllers;


[Authorize]
[Route("[controller]/[action]")]
[ApiController]
public class LoginController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly ILogger<LoginController> _logger;
    private readonly IDatabaseService _dbservice;
    private const string _DUMMY_PASSWORD_HASH_ = "Rfc2898DeriveBytes$50000$N0GUkDVktle2QHUiortgZw==$Q8mijJ2JTMhAtuCf4indiaXZKQe6XWLUjLyxnuyl8Gg=";

    public LoginController(IConfiguration config, ILogger<LoginController> logger, IDatabaseService dbservice)
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
        UserDTO? user = Authenticate(username, password);
        if (user == null)
        {
            Response.Cookies.Delete("access_token");
            return Unauthorized("Username or password is incorrect");
        }

        int expiryMinutes = 60;
        List<Claim> claims = GenerateClaims(user);

        // Cookie authentication (.Net cookie)
        await SetAuthenticationCookie(claims, expiryMinutes);

        // JWT bearer token authentication
        string token = GenerateToken(claims, expiryMinutes);

        _logger.LogInformation("User logged in: {User}", user.ToString());
        return Ok(token);
    }

    [HttpGet]
    [ApiExplorerSettings(IgnoreApi = true)]  // FixMe: Is this still used by the mobile app?
    public async Task<IActionResult> GetUser(int uid)
    {
        UserDTO? authuser = _dbservice.GetUserById(uid, true);

        return Ok(authuser);
    }

    [HttpGet]
    public async Task<IActionResult> Me()
    {
        Int32.TryParse(User.FindFirstValue(ClaimTypes.Upn), out int uid);

        return GetUser(uid).Result;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CurrentUser()
    {
        string authusername = User.FindFirstValue(ClaimTypes.NameIdentifier);

        UserDTO? authuser = _dbservice.GetUserByUsername(authusername);

        return Ok(authuser);
    }

    // Workaround endpoint to mimick a 401 Unaothorized when .Net authorization cookie is missing
    [ApiExplorerSettings(IgnoreApi = true)]
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> Status_401()
    {
        return StatusCode(401, "Unathorized");
    }

    private async Task SetAuthenticationCookie(List<Claim> claims, int expiryMinutes)
    {
        var claimsIdentity = new ClaimsIdentity(
            claims, CookieAuthenticationDefaults.AuthenticationScheme);

        var authProperties = new AuthenticationProperties
        {
            ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(expiryMinutes),
            IsPersistent = true,
            IssuedUtc = DateTimeOffset.UtcNow,
        };

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);
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
        }
        else if (pbkdf.Verify(password, _config["Admin:MasterPassword"]))
        {
            return user;
        }
        else
        {
            return null;
        }
    }

    private List<Claim> GenerateClaims(UserDTO user)
    {
        List<Claim> claims = new()
        {
            new Claim(ClaimTypes.Upn, user.Id.ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.UserName),
            new Claim("FullName", user.Name),
            new Claim(ClaimTypes.Name, user.Name)
        };
        return claims;
    }

    private string GenerateToken(List<Claim> claims, int expiryMinutes)
    {

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:SecretKey"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            _config["Jwt:Issuer"],
            _config["Jwt:Audience"],
            claims,
            expires: DateTime.Now.AddMinutes(expiryMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
