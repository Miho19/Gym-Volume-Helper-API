
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
    public async Task GetMe_JWTMissingSubWithValidUser_Return404Failure()
    {
        _userRepositoryMock.Setup(x => x.GetUser(It.IsAny<string>())).ReturnsAsync(UserFixture.TestUser());
        _userControllerMock.ControllerContext = new ControllerContext() { HttpContext = UserFixture.TestInvalidUserHttpContext() };

        var result = await _userControllerMock.GetMe();
        Assert.NotNull(result);
        var notFoundObjectResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(StatusCodes.Status404NotFound, notFoundObjectResult.StatusCode);
    }



}