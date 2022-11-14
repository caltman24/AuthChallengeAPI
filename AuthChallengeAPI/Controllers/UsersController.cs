using AuthChallengeAPI.Constants;
using AuthChallengeAPI.Models;
using AuthChallengeAPI.TestData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthChallengeAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IConfiguration _config;

    public UsersController(IConfiguration config)
    {
        _config = config;
    }
    
    [HttpGet]
    public ActionResult<List<UserModel>> GetAll()
    {
        var users = FakeData.AllUsers;
        return Ok(users);
    }

    [HttpGet("{id:int}")]
    [Authorize(Policy = PolicyConstants.MustBeTheOwner)]
    public ActionResult<UserModel> GetById(int id)
    {
        var user = FakeData.GetUserById(id);

        if (user is null)
        {
            return NotFound("User Not Found");
        }
        
        return Ok(_config.GetValue<string>("ConnectionStrings:Default"));
    }
}