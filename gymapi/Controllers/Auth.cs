namespace gymapi.Controllers;

using gymapi.Data;
using gymapi.Models;
using gymapi.src.AuthManagement;
using gymapi.src.AuthManagement.Auth0;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly ILogger<AuthController> _logger;
    private readonly IUserRepository _userRepository;
    private readonly IAuthManagement _authManagement;

    public AuthController(ILogger<AuthController> logger, IUserRepository userRepository, IAuthManagement authManagement)
    {
        _logger = logger;
        _userRepository = userRepository;
        _authManagement = authManagement;
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

            var createdUser = await CreateUser(bodyRequest.AuthID);
            return CreatedAtAction(nameof(PostCreateUser), new { id = createdUser.Id }, createdUser);

        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Error creating new user\n{e.Message}");
        }

    }

    private async Task<User> CreateUser(string userID)
    {

        var authenicatedUser = await AuthenicateUser(userID) as Auth0ManagementSystemFetchUserResponse;
        if (authenicatedUser is null) throw new Exception("Failed to authenicate new user");
        var newUser = Auth0ManagementSystemFetchUserResponseToUserDto(authenicatedUser);
        await _userRepository.AddUser(newUser);

        return newUser;
    }

    private async Task<IAuthManagementFetchUserResponse> AuthenicateUser(string userID)
    {
        await _authManagement.Initialise();
        return await _authManagement.FetchUser(userID);
    }

    private User Auth0ManagementSystemFetchUserResponseToUserDto(Auth0ManagementSystemFetchUserResponse fetchUserResponse)
    {

        if (String.IsNullOrEmpty(fetchUserResponse.UserID)) throw new Exception("Auth0 fetch user response missing userID");
        if (String.IsNullOrEmpty(fetchUserResponse.Image)) throw new Exception("Auth0 fetch user response missing picture source");

        return new User()
        {
            Id = fetchUserResponse.UserID,
            CurrentWorkoutId = "0",
            Weight = 0,
            PictureSource = fetchUserResponse.Image,
        };
    }
}





