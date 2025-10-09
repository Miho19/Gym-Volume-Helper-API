namespace gymapi.Data.Repository.User;

using System.Collections.Generic;
using System.Threading.Tasks;
using gymapi.Models;
using Microsoft.EntityFrameworkCore;

public class UserRepository : IUserRepository
{
    private readonly GymServerContext _context;


    public UserRepository(GymServerContext context)
    {
        _context = context;

    }

    public async Task AddUser(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteUser(string userId)
    {
        var user = await _context.Users.FirstOrDefaultAsync<User>(u => u.Id == userId);
        if (user is null) return;
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
    }

    public async Task<User?> GetUser(string userId)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
    }

    public async Task<IEnumerable<User>> GetUsers()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task<User?> UpdateUser(User user)
    {
        var result = await _context.Users.FirstOrDefaultAsync<User>(u => u.Id == user.Id);
        if (result == null) return null;

        result.PictureSource = user.PictureSource;
        result.Weight = user.Weight;
        result.CurrentWorkoutId = user.CurrentWorkoutId;

        await _context.SaveChangesAsync();

        return result;

    }
}