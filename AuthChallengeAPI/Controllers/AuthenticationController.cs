using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthChallengeAPI.Models;
using AuthChallengeAPI.TestData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace AuthChallengeAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly IConfiguration _config;

    public AuthenticationController(IConfiguration config)
    {
        _config = config;
    }

    [HttpPost("token")]
    [AllowAnonymous]
    public IActionResult Authenticate([FromBody] LoginModel loginData)
    {
        var user = ValidateCredentials(loginData); // A 3rd party service will replace this

        if (user is null)
        {
            return Unauthorized();
        }

        var token = GenerateToken(user);

        return Ok(token);
    }

    private string GenerateToken(UserModel user)
    {
        var secretKey = new SymmetricSecurityKey(
            Encoding.ASCII.GetBytes(_config.GetValue<string>("Authentication:SecretKey")));

        var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

        List<Claim> claims = new()
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
            new Claim("title", user.Title)
        };

        var token = new JwtSecurityToken(
            _config.GetValue<string>("Authentication:Issuer"),
            _config.GetValue<string>("Authentication:Audience"),
            claims,
            DateTime.UtcNow,
            DateTime.UtcNow.AddMinutes(1),
            signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
    private static UserModel? ValidateCredentials(LoginModel loginData)
    {
        // DO NOT USE IN PRODUCTION - DEMO CODE ONLY
        var user = FakeData.GetUserByUserName(loginData.UserName!);

        if (user is not null && CompareValues(loginData.Password, "password"))
        {
            return user;
        }

        return null;
    }

    private static bool CompareValues(string? s1, string s2)
    {
        return s1 is not null && s1.Equals(s2, StringComparison.InvariantCultureIgnoreCase);
    }
}