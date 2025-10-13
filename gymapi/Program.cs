using System.Security.Claims;
using dotenv.net;
using gymapi.Data;
using gymapi.Data.Repository.User;
using Microsoft.AspNetCore.Authentication.JwtBearer;


var builder = WebApplication.CreateBuilder(args);

DotEnv.Load();



// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// dependency injection stuff
builder.Services.AddDbContext<GymServerContext>();
builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddHttpClient();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.Authority = $"{System.Environment.GetEnvironmentVariable("Auth0URL")}";
    options.Audience = $"{System.Environment.GetEnvironmentVariable("Auth0ManagementAPIIdentifier")}";
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
    {
        NameClaimType = ClaimTypes.NameIdentifier
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
