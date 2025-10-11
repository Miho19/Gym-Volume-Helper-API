namespace gymapi.Controllers;

using gymapi.Data;
using gymapi.Models;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly ILogger<AuthController> _logger;
    private readonly IUserRepository _userRepository;


    public AuthController(ILogger<AuthController> logger, IUserRepository userRepository)
    {
        _logger = logger;
        _userRepository = userRepository;
    }

    [HttpPost]
    public async Task<IActionResult> PostCreateUser([FromBody] PostAuthBodyRequestBody? bodyRequest)
    {
        try
        {
            if (bodyRequest is null) return BadRequest();
            if (String.IsNullOrEmpty(bodyRequest.AuthID)) return BadRequest();

            var user = await _userRepository.GetUser(bodyRequest.AuthID);
            if (user is not null) return BadRequest("User already exists");


            return CreatedAtAction(nameof(PostCreateUser), new { id = "1" }, new User { Id = "", CurrentWorkoutId = "", PictureSource = "", Weight = 0 });

        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Error creating new user\n{e.Message}");
        }

    }




}





