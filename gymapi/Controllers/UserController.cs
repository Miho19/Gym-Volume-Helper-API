
using System.IdentityModel.Tokens.Jwt;
using gymapi.Data;
using gymapi.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace gymapi.Controllers;

/** 
    Get, Post

    /user

    Get, Patch/Put/Delete?
    /user/me
    /user/{id:int}


*/

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly IUserRepository _userRepository;
    public UserController(ILogger<UserController> logger, IUserRepository userRepository)
    {
        _logger = logger;
        _userRepository = userRepository;
    }

    [HttpGet("GetMe")]
    public async Task<IActionResult> GetMe()
    {
        var userSub = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

        if (string.IsNullOrEmpty(userSub))
            return new UnauthorizedObjectResult("JWT missing Sub claim");

        var user = await _userRepository.GetUser(userSub);
        if (user is not null)
            return new OkObjectResult(user);

        return new RedirectResult("/createnewuser");
    }

    [HttpPost("CreateMe")]
    public async Task<IActionResult> CreateMe([FromBody] User requestBody)
    {
        var userSub = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (string.IsNullOrEmpty(userSub))
            return new UnauthorizedObjectResult("JWT missing Sub claim");

        var user = await _userRepository.GetUser(userSub);
        if (user is not null)
            return new BadRequestObjectResult("User already exists");

        await _userRepository.AddUser(requestBody);

        return new CreatedAtActionResult("Created user", nameof(CreateMe), new { Id = requestBody.Id }, requestBody);

    }

}