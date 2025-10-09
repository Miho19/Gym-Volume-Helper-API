using gymapi.Models;
using gymapi.src.AuthManagement.Auth0;

namespace gymapi.UnitTests.Fixtures;

public static class UserFixture
{
    public static User TestUser()
    {
        return new User()
        {
            Id = "123456",
            PictureSource = "https://fakeImageUrl.com",
            CurrentWorkoutId = "0",
            Weight = 0,
        };
    }

    public static List<User> TestUsers()
    {
        return new List<User>()
        {
            new User(){ Id="1", PictureSource="https://fakeImageUrl.com/1", CurrentWorkoutId="1", Weight=60},
            new User(){ Id="2", PictureSource="https://fakeImageUrl.com/2", CurrentWorkoutId="2", Weight=79},
            new User(){ Id="3", PictureSource="hhttps://fakeImageUrl.com/3", CurrentWorkoutId="3", Weight=55}
        };
    }

    public static Auth0ManagementSystemFetchUserResponse TestAuth0UserResponse()
    {
        return new Auth0ManagementSystemFetchUserResponse()
        {
            UserID = "123456",
            Image = "https://fakeImageUrl.com"
        };
    }
}