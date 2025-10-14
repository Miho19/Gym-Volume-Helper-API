
using gymapi.Controllers;
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
    private readonly ITestOutputHelper _logger;

    public UserControllerTest(ITestOutputHelper logger)
    {
        _logger = logger;
        var loggerMock = new Mock<ILogger<UserController>>();
        _userControllerMock = new UserController(loggerMock.Object);
    }

    [Fact]
    public async Task GetMe_OnSuccess_Returns200StatusCode()
    {
        _userControllerMock.ControllerContext = new ControllerContext() { HttpContext = UserFixture.TestValidUserHttpContext() };

        var result = await _userControllerMock.GetMe();
        Assert.NotNull(result);
        var okResultObject = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(StatusCodes.Status200OK, okResultObject.StatusCode);
    }
    [Fact]
    public async Task GetMe_OnFailure_Returns404StatusCode()
    {
        _userControllerMock.ControllerContext = new ControllerContext() { HttpContext = UserFixture.TestInvalidUserHttpContext() };

        var result = await _userControllerMock.GetMe();
        Assert.NotNull(result);
        var notFoundObjectResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(StatusCodes.Status404NotFound, notFoundObjectResult.StatusCode);
    }



}