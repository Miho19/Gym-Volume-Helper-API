using dotenv.net;
using gymapi.Models;
using Microsoft.EntityFrameworkCore;

namespace gymapi.Data;


public class GymServerContext : DbContext
{
    public virtual DbSet<User> Users { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        DotEnv.Load();
        optionsBuilder.UseMySql(Environment.GetEnvironmentVariable("DatabaseConnectionString"), new MySqlServerVersion(new Version(8, 0, 43))).EnableDetailedErrors();
    }
}