using BadmintonClubAPI.Data;
using BadmintonClubAPI.Models.Entities;
using BadmintonClubAPI.Services;
using BadmintonClubAPI.Services.CourtBookings;
using BadmintonClubAPI.Services.Players;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}