
namespace gymapi.UnitTests.Systems.Controllers;

using gymapi.Controllers;
using gymapi.Data;
using gymapi.Data.Repository.User;
using gymapi.Models;
using gymapi.UnitTests.Fixtures;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Xunit.Abstractions;

public class UserControllerTest
{

    private readonly UserController _userControllerMock;
    private readonly Mock<gymapi.Data.IUserRepository> _userRepositoryMock;
    private readonly ITestOutputHelper _logger;

    public UserControllerTest(ITestOutputHelper logger)
    {
        _logger = logger;
        var loggerMock = new Mock<ILogger<UserController>>();
        var userRepositoryMock = new Mock<IUserRepository>();
        _userRepositoryMock = userRepositoryMock;
        _userControllerMock = new UserController(loggerMock.Object, userRepositoryMock.Object);
    }

    [Fact]
    public async Task GetMe_JWTHasSubWithValidUser_Returns200StatusCode()
    {
        _userRepositoryMock.Setup(x => x.GetUser(It.IsAny<string>())).ReturnsAsync(UserFixture.TestUser());
        _userControllerMock.ControllerContext = new ControllerContext() { HttpContext = UserFixture.TestValidUserHttpContext() };
        var result = await _userControllerMock.GetMe();

        Assert.NotNull(result);
        var okResultObject = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(StatusCodes.Status200OK, okResultObject.StatusCode);
    }



    [Fact]
    public async Task GetMe_JWTHasSubWithInvalidUser_Returns302RedirectStatusCode()
    {
        _userRepositoryMock.Setup(x => x.GetUser(It.IsAny<string>())).ReturnsAsync((User?)null);
        _userControllerMock.ControllerContext = new ControllerContext() { HttpContext = UserFixture.TestValidUserHttpContext() };
        var result = await _userControllerMock.GetMe();

        Assert.NotNull(result);

        var redirectResult = Assert.IsType<RedirectResult>(result);
        Assert.Equal("/createnewuser", redirectResult.Url);
    }


    [Fact]
    public async Task GetMe_JWTMissingSubWithValidUser_Return401UnauthorizedStatusCode()
    {
        _userRepositoryMock.Setup(x => x.GetUser(It.IsAny<string>())).ReturnsAsync(UserFixture.TestUser());
        _userControllerMock.ControllerContext = new ControllerContext() { HttpContext = UserFixture.TestInvalidUserHttpContext() };

        var result = await _userControllerMock.GetMe();
        Assert.NotNull(result);
        var unauthorizedObjectResult = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.Equal(StatusCodes.Status401Unauthorized, unauthorizedObjectResult.StatusCode);
    }

    [Fact]
    public async Task CreateMe_OnSuccess_WithValidSubAndRequestUserBody_Returns201CreatedStatusCode()
    {
        _userControllerMock.ControllerContext = new ControllerContext() { HttpContext = UserFixture.TestValidUserHttpContext() };

        var result = await _userControllerMock.CreateMe(UserFixture.TestUser());

        Assert.NotNull(result);
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(StatusCodes.Status201Created, createdAtActionResult.StatusCode);
    }

    [Fact]
    public async Task CreateMe_OnSuccess_WithValidSubAndRequestUserBody_ReturnedCreatedUserHasSamePropertiesAsRequestBody()
    {
        _userControllerMock.ControllerContext = new ControllerContext() { HttpContext = UserFixture.TestValidUserHttpContext() };

        var requestBody = UserFixture.TestUser();

        var result = await _userControllerMock.CreateMe(requestBody);

        Assert.NotNull(result);
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
        var createdUser = Assert.IsType<User>(createdAtActionResult.Value);

        Assert.Equal(requestBody.Id, createdUser.Id);
        Assert.Equal(requestBody.PictureSource, createdUser.PictureSource);
        Assert.Equal(requestBody.Weight, createdUser.Weight);
        Assert.Equal(requestBody.CurrentWorkoutId, createdUser.CurrentWorkoutId);
    }
    [Fact]
    public async Task CreateMe_OnFailure_WithInvalidSubAndRequestUserBody_Returns404NotFound()
    {
        _userControllerMock.ControllerContext = new ControllerContext() { HttpContext = UserFixture.TestInvalidUserHttpContext() };

        var requestBody = UserFixture.TestUser();

        var result = await _userControllerMock.CreateMe(requestBody);

        Assert.NotNull(result);
        var unauthorizedObjectResult = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.Equal(StatusCodes.Status401Unauthorized, unauthorizedObjectResult.StatusCode);
    }
    [Fact]
    public async Task CreateMe_OnFailure_WithValidSubAndRequestUserBodyWhenUserAlreadyExists_Returns400BadRequest()
    {
        _userControllerMock.ControllerContext = new ControllerContext() { HttpContext = UserFixture.TestValidUserHttpContext() };
        _userRepositoryMock.Setup(x => x.GetUser(It.IsAny<string>())).ReturnsAsync(UserFixture.TestUser());

        var requestBody = UserFixture.TestUser();

        var result = await _userControllerMock.CreateMe(requestBody);

        Assert.NotNull(result);
        var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(StatusCodes.Status400BadRequest, badRequestObjectResult.StatusCode);
    }

    [Fact]
    public async Task CreateMe_OnSuccess_WithValidSubAndRequestUserBody_CallsCorrectMethods()
    {
        _userControllerMock.ControllerContext = new ControllerContext() { HttpContext = UserFixture.TestValidUserHttpContext() };
        var requestBody = UserFixture.TestUser();
        var result = await _userControllerMock.CreateMe(requestBody);

        Assert.NotNull(result);
        Assert.IsType<CreatedAtActionResult>(result);


        _userRepositoryMock.Verify(x => x.GetUser(It.IsAny<string>()), Times.Exactly(1));
        _userRepositoryMock.Verify(x => x.AddUser(It.IsAny<User>()), Times.Exactly(1));

    }



}