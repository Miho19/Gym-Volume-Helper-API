namespace gymapi.UnitTests.Systems.Repository;

using gymapi.Data;
using gymapi.Data.Repository.User;
using gymapi.Models;
using gymapi.UnitTests.Fixtures;
using Microsoft.EntityFrameworkCore;

using Moq;
using Moq.EntityFrameworkCore;
using Xunit;
using MockQueryable.Core;
using MockQueryable.Moq;
using MockQueryable.EntityFrameworkCore;
using System.Linq.Expressions;

public class UserRepositoryTest
{
    [Fact]
    public async Task GetUsers_ReturnsListOfUsers()
    {



        var users = UserFixture.TestUsers();
        var usersDbsetMock = users.BuildMockDbSet();

        var gymServerContextMock = new Mock<GymServerContext>();
        gymServerContextMock.Setup(m => m.Users).ReturnsDbSet(usersDbsetMock.Object);

        var userRepositoryMock = new UserRepository(gymServerContextMock.Object);

        var result = await userRepositoryMock.GetUsers();

        Assert.Equal(UserFixture.TestUsers().Count, result.ToList().Count);

    }

    [Theory]
    [InlineData("1")]
    [InlineData("2")]
    [InlineData("3")]
    public async Task GetUser_ReturnsSpecificUserByTheirId(string userID)
    {
        var userDataMock = UserFixture.TestUsers();

        var gymServerContextMock = new Mock<GymServerContext>();
        gymServerContextMock.Setup(m => m.Users).ReturnsDbSet(userDataMock);

        var userRepositoryMock = new UserRepository(gymServerContextMock.Object);

        var result = await userRepositoryMock.GetUser(userID);

        Assert.NotNull(result);
        Assert.Equal(userID, result.Id);
    }

    [Theory]
    [InlineData("-1")]
    [InlineData("-1000")]
    [InlineData("1000000000000000000")]
    public async Task GetUser_ReturnsNull_WhenUserIdIsNotFound(string userID)
    {
        var userDataMock = UserFixture.TestUsers();

        var gymServerContextMock = new Mock<GymServerContext>();
        gymServerContextMock.Setup(m => m.Users).ReturnsDbSet(userDataMock);

        var userRepositoryMock = new UserRepository(gymServerContextMock.Object);

        var result = await userRepositoryMock.GetUser(userID);

        Assert.Null(result);

    }

    [Fact]
    public async Task AddUser_AddingUser_CorrectMethodsCalled()
    {
        var users = UserFixture.TestUsers();
        var usersDbsetMock = new Mock<DbSet<User>>();

        var newUser = new User()
        {
            Id = "4",
            CurrentWorkoutId = "0",
            PictureSource = "Fake Image Source",
            Weight = 0
        };

        var gymServerContextMock = new Mock<GymServerContext>();
        gymServerContextMock.Setup(m => m.Users).Returns(usersDbsetMock.Object);

        var userRepositoryMock = new UserRepository(gymServerContextMock.Object);



        await userRepositoryMock.AddUser(newUser);

        usersDbsetMock.Verify(x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
        gymServerContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

    }

    [Theory]
    [InlineData("1")]
    public async Task DeleteUser_CallsTheCorrectMethods(string userID)
    {

        var users = UserFixture.TestUsers();
        var usersDbsetMock = users.BuildMockDbSet();

        var gymServerContextMock = new Mock<GymServerContext>();
        gymServerContextMock.Setup(m => m.Users).Returns(usersDbsetMock.Object);



        var userRepositoryMock = new UserRepository(gymServerContextMock.Object);



        await userRepositoryMock.DeleteUser(userID);

        usersDbsetMock.Verify(x => x.Remove(It.IsAny<User>()), Times.Once);
        gymServerContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

    }

    [Fact]
    public async Task UpdateUser_ReturnsUpdatedUser()
    {

        var users = UserFixture.TestUsers();
        var usersDbsetMock = users.BuildMockDbSet();

        var gymServerContextMock = new Mock<GymServerContext>();
        gymServerContextMock.Setup(m => m.Users).Returns(usersDbsetMock.Object);

        var userRepositoryMock = new UserRepository(gymServerContextMock.Object);

        var updateUser = new User()
        {
            Id = users[0].Id,
            PictureSource = "Updated Picture Source",
            Weight = users[0].Weight,
            CurrentWorkoutId = users[0].CurrentWorkoutId
        };


        var result = await userRepositoryMock.UpdateUser(updateUser);


        // usersDbsetMock.Verify(x => x.Remove(It.IsAny<User>()), Times.Once);
        // gymServerContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        Assert.NotNull(result);
        Assert.IsType<User>(result);
        Assert.Equal(users[0].Id, result.Id);
        Assert.Equal(users[0].PictureSource, result.PictureSource);
        Assert.Equal(users[0].Weight, result.Weight);
        Assert.Equal(users[0].CurrentWorkoutId, result.CurrentWorkoutId);

    }

    /** 
        Useless test --> wanted to verify that firstordefault was called but can not
    */
    [Fact]
    public async Task UpdateUser_CallsCorrectMethods()
    {

        var users = UserFixture.TestUsers();
        var usersDbsetMock = users.BuildMockDbSet();

        var gymServerContextMock = new Mock<GymServerContext>();
        gymServerContextMock.Setup(m => m.Users).Returns(usersDbsetMock.Object);

        var userRepositoryMock = new UserRepository(gymServerContextMock.Object);

        var updateUser = new User()
        {
            Id = users[0].Id,
            PictureSource = "Updated Picture Source",
            Weight = users[0].Weight,
            CurrentWorkoutId = users[0].CurrentWorkoutId
        };


        var result = await userRepositoryMock.UpdateUser(updateUser);
        gymServerContextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateUser_ReturnNull_WhenGivenUserThatDoesNotExist()
    {

        var users = UserFixture.TestUsers();
        var usersDbsetMock = users.BuildMockDbSet();

        var gymServerContextMock = new Mock<GymServerContext>();
        gymServerContextMock.Setup(m => m.Users).Returns(usersDbsetMock.Object);

        var userRepositoryMock = new UserRepository(gymServerContextMock.Object);

        var updateUser = new User()
        {
            Id = "-1",
            PictureSource = "Updated Picture Source",
            Weight = users[0].Weight,
            CurrentWorkoutId = users[0].CurrentWorkoutId
        };

        var result = await userRepositoryMock.UpdateUser(updateUser);

        Assert.Null(result);

    }
}