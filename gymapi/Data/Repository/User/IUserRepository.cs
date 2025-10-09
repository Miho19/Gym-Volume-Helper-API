using gymapi.Models;

namespace gymapi.Data;


public interface IUserRepository
{
    Task<IEnumerable<User>> GetUsers();
    Task<User?> GetUser(string userId);
    Task AddUser(User user);
    Task<User?> UpdateUser(User user);
    Task DeleteUser(string userId);

}