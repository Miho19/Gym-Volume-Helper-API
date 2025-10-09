using gymapi.Controllers;
using gymapi.Data;
using gymapi.Data.Repository.User;
using gymapi.src.AuthManagement;
using gymapi.src.AuthManagement.Auth0;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// dependency injection stuff
builder.Services.AddDbContext<GymServerContext>();
builder.Services.AddTransient<IUserRepository, UserRepository>();
// builder.Services.AddScoped<IUserService, UserService>();
// builder.Services.Configure<UsersApiOptions>(builder.Configuration.GetSection("UsersApiOptions"));
builder.Services.AddHttpClient();
builder.Services.AddSingleton<IAuthManagement, Auth0Management>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
