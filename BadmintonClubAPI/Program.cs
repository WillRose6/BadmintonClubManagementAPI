using System.Text;
using BadmintonClubAPI.Data;
using BadmintonClubAPI.Models.Entities;
using BadmintonClubAPI.Services.CourtBookings;
using BadmintonClubAPI.Services.Leagues;
using BadmintonClubAPI.Services.Matches;
using BadmintonClubAPI.Services.Players;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace BadmintonClubAPI;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddDbContext<BadmintonDbContext>(options => options.UseSqlite("Data Source=Badminton.db"));
        
        // Dependency injection
        builder.Services.AddScoped<IPlayerService, PlayerService>();
        builder.Services.AddScoped<IPasswordHasher<Player>, PasswordHasher<Player>>();
        builder.Services.AddScoped<ICourtBookingsService, CourtBookingsService>();
        builder.Services.AddScoped<ILeagueService, LeagueService>();
        builder.Services.AddScoped<IMatchService, MatchService>();

        builder.Services.AddFluentValidationAutoValidation();
        builder.Services.AddValidatorsFromAssemblyContaining<Program>();

        var jwtKey = builder.Configuration["Jwt:Key"] ?? "SuperSecretKey12345678901234567890!";
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "BadmintonAPI",
                    ValidAudience = builder.Configuration["Jwt:Audience"] ?? "BadmintonAPI",
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                };
            });
        
        builder.Services.AddControllers();
        
        // Add services to the container.
        builder.Services.AddAuthorization();

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();
        
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
    }
}