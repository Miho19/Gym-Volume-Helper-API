
using System.IdentityModel.Tokens.Jwt;
using gymapi.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace gymapi.Controllers;


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
            return NotFound("User does not exist");

        var user = await _userRepository.GetUser(userSub);
        if (user is not null)
            return Ok(user);

        return StatusCode(500, "An unexpected server error occurred");
    }
}