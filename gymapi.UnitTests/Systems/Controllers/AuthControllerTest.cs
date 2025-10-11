// namespace gymapi.UnitTests.Systems.Controllers;

// using System.Net;
// using gymapi.Controllers;
// using gymapi.Data;
// using gymapi.Models;
// using gymapi.src.AuthManagement;
// using gymapi.UnitTests.Fixtures;
// using Microsoft.AspNetCore.Http;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.Extensions.Logging;
// using Moq;
// using Xunit;
// using Xunit.Abstractions;

// public class AuthControllerTest
// {
//     private readonly ITestOutputHelper _logger;

//     public AuthControllerTest(ITestOutputHelper logger)
//     {
//         _logger = logger;
//     }
//     [Fact]
//     public async Task POST_CreateUser_OnSuccess_ReturnsStatusCode201_ForNewUser()
//     {
//         var loggerMock = new Mock<ILogger<AuthController>>();

//         var userRepositoryMock = new Mock<IUserRepository>();
//         userRepositoryMock.Setup(u => u.GetUser(It.IsAny<String>())).ReturnsAsync((User?)null);


//         var authManagementMock = new Mock<IAuthManagement>();
//         authManagementMock.Setup(a => a.FetchUser(It.IsAny<string>())).ReturnsAsync(UserFixture.TestAuth0UserResponse);


//         var postBodyRequest = new PostAuthBodyRequestBody() { AuthID = "123456" };

//         var authController = new AuthController(loggerMock.Object, userRepositoryMock.Object, authManagementMock.Object);
//         var result = await authController.PostCreateUser(postBodyRequest);

//         Assert.NotNull(result);
//         var createdAtActionResultObject = Assert.IsType<CreatedAtActionResult>(result);
//         Assert.Equal(StatusCodes.Status201Created, createdAtActionResultObject.StatusCode);

//     }
//     [Fact]
//     public async Task POST_CreateUser_ReturnsStatusCode400_IfUserExists()
//     {
//         var loggerMock = new Mock<ILogger<AuthController>>();

//         var userRepositoryMock = new Mock<IUserRepository>();
//         userRepositoryMock.Setup(u => u.GetUser(It.IsAny<String>())).ReturnsAsync(UserFixture.TestUser);


//         var authManagementMock = new Mock<IAuthManagement>();



//         var postBodyRequest = new PostAuthBodyRequestBody() { AuthID = "123456" };

//         var authController = new AuthController(loggerMock.Object, userRepositoryMock.Object, authManagementMock.Object);
//         var result = await authController.PostCreateUser(postBodyRequest);

//         Assert.NotNull(result);
//         var badRequestObject = Assert.IsType<BadRequestObjectResult>(result);
//         Assert.Equal(StatusCodes.Status400BadRequest, badRequestObject.StatusCode);

//     }

//     [Fact]
//     public async Task POST_CreateUser_OnFailure_ReturnsStatusCode400_ForNoBody()
//     {
//         var loggerMock = new Mock<ILogger<AuthController>>();
//         var userRepositoryMock = new Mock<IUserRepository>();
//         var authManagementMock = new Mock<IAuthManagement>();

//         var authController = new AuthController(loggerMock.Object, userRepositoryMock.Object, authManagementMock.Object);
//         var result = await authController.PostCreateUser(null);

//         Assert.NotNull(result);
//         var badRequestObject = Assert.IsType<BadRequestResult>(result);
//         Assert.Equal(StatusCodes.Status400BadRequest, badRequestObject.StatusCode);

//     }

//     [Fact]
//     public async Task POST_CreateUser_OnFailure_ReturnsStatusCode400_BodyIsMissingAuthID()
//     {
//         var loggerMock = new Mock<ILogger<AuthController>>();
//         var userRepositoryMock = new Mock<IUserRepository>();
//         var authManagementMock = new Mock<IAuthManagement>();

//         var authController = new AuthController(loggerMock.Object, userRepositoryMock.Object, authManagementMock.Object);
//         var result = await authController.PostCreateUser(new PostAuthBodyRequestBody() { });

//         Assert.NotNull(result);
//         var badRequestObject = Assert.IsType<BadRequestResult>(result);
//         Assert.Equal(StatusCodes.Status400BadRequest, badRequestObject.StatusCode);

//     }
// }