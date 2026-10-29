using BadmintonClubAPI.Data;
using BadmintonClubAPI.Models.DTOs.Leagues;
using BadmintonClubAPI.Models.Entities;
using BadmintonClubAPI.Services.Leagues;
using Microsoft.EntityFrameworkCore;

namespace BadmintonClubAPI.Tests.Services;

public class LeagueServiceTests
{
    private BadmintonDbContext GetInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<BadmintonDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new BadmintonDbContext(options);
    }

    [Fact]
    public async Task GetAllLeaguesAsync_ShouldReturnAllLeagues()
    {
        await using var context = GetInMemoryContext();
        context.Leagues.AddRange(
            new League { Name = "Super Series Division 1", StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddMonths(3) },
            new League { Name = "County Cup", StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddMonths(3) }
        );
        await context.SaveChangesAsync();

        var service = new LeagueService(context);
        var result = await service.GetAllLeaguesAsync();

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task CreateLeague_ShouldAddLeagueToDatabase()
    {
        await using var context = GetInMemoryContext();
        var service = new LeagueService(context);
        var dto = new CreateLeagueDto("Spring Shuttle Cup", DateTime.UtcNow, DateTime.UtcNow.AddMonths(2));

        var result = await service.CreateLeague(dto);

        Assert.NotNull(result);
        Assert.Equal("Spring Shuttle Cup", result.Name);
        Assert.Single(context.Leagues);
    }

    [Fact]
    public async Task AddPlayerToLeague_WhenValid_ShouldReturnSuccess()
    {
        await using var context = GetInMemoryContext();
        var league = new League { Name = "Division 1", StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddMonths(3) };
        var player = new Player("Daniella", "Smith", "Daniella.Smith@gmail.com", new DateTime(2000, 5, 20));

        context.Leagues.Add(league);
        context.Players.Add(player);
        await context.SaveChangesAsync();

        var service = new LeagueService(context);
        var result = await service.AddPlayerToLeague(league.Id, player.Id);

        Assert.Equal(LeagueResult.Success, result);
        Assert.Single(league.Players);
    }

    [Fact]
    public async Task AddPlayerToLeague_WhenAlreadyInLeague_ShouldReturnPlayerAlreadyInLeague()
    {
        await using var context = GetInMemoryContext();
        var player = new Player("Daniella", "Smith", "Daniella.Smith@gmail.com", new DateTime(2000, 5, 20));
        var league = new League 
        { 
            Name = "Division 1", 
            StartDate = DateTime.UtcNow, 
            EndDate = DateTime.UtcNow.AddMonths(3),
            Players = new List<Player> { player }
        };

        context.Leagues.Add(league);
        await context.SaveChangesAsync();

        var service = new LeagueService(context);
        var result = await service.AddPlayerToLeague(league.Id, player.Id);

        Assert.Equal(LeagueResult.PlayerAlreadyInLeague, result);
    }

    [Fact]
    public async Task RemovePlayerFromLeague_WhenPlayerInLeague_ShouldReturnSuccess()
    {
        await using var context = GetInMemoryContext();
        var player = new Player("Daniella", "Smith", "Daniella.Smith@gmail.com", new DateTime(2000, 5, 20));
        var league = new League 
        { 
            Name = "Division 1", 
            StartDate = DateTime.UtcNow, 
            EndDate = DateTime.UtcNow.AddMonths(3),
            Players = new List<Player> { player }
        };

        context.Leagues.Add(league);
        await context.SaveChangesAsync();

        var service = new LeagueService(context);
        var result = await service.RemovePlayerFromLeague(league.Id, player.Id);

        Assert.Equal(LeagueResult.Success, result);
        Assert.Empty(league.Players);
    }
}