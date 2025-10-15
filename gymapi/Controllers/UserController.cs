
using System.IdentityModel.Tokens.Jwt;
using gymapi.Data;
using gymapi.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;


namespace gymapi.Controllers;

/** 
    Post -> Create new user
    /user

    Get -> current user, PUT PATCH DELETE
    /user/me

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
    [Route("/me")]
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

    [HttpPut]
    public async Task<IActionResult> UpdateMe([FromBody] User requestBody)
    {
        var userSub = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (string.IsNullOrEmpty(userSub))
            return new UnauthorizedObjectResult("JWT missing Sub claim");

        if (requestBody.Id != userSub)
            return Forbid();

        var result = await _userRepository.UpdateUser(requestBody);
        if (result is null)
            return new NotFoundObjectResult("User not found");

        return new OkObjectResult(result);
    }

}