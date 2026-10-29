using BadmintonClubAPI.Data;
using BadmintonClubAPI.Models.DTOs.Matches;
using BadmintonClubAPI.Models.Entities;
using BadmintonClubAPI.Services.Matches;
using Microsoft.EntityFrameworkCore;

namespace BadmintonClubAPI.Tests.Services;

public class MatchServiceTests
{
    private BadmintonDbContext GetInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<BadmintonDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new BadmintonDbContext(options);
    }

    [Fact]
    public async Task CreateMatchAsync_WhenPlayersExist_ShouldReturnSuccess()
    {
        await using var context = GetInMemoryContext();
        var player1 = new Player("Daniella", "Smith", "Daniella.Smith@gmail.com", new DateTime(2000, 5, 20));
        var player2 = new Player("Alex", "Price", "Alex.Price@gmail.com", new DateTime(1995, 2, 18));
        
        context.Players.AddRange(player1, player2);
        await context.SaveChangesAsync();

        var service = new MatchService(context);
        var dto = new CreateMatchDto(
            DateTime.UtcNow, 
            DateTime.UtcNow.AddHours(1), 
            null, 
            new List<MatchParticipantDto>
            {
                new(player1.Id, 1, true),
                new(player2.Id, 2, false)
            });

        var (result, match) = await service.CreateMatchAsync(dto);

        Assert.Equal(MatchResult.Success, result);
        Assert.NotNull(match);
        Assert.Equal(2, match.Participants.Count);
    }

    [Fact]
    public async Task CreateMatchAsync_WhenPlayerDoesNotExist_ShouldReturnPlayerNotFound()
    {
        await using var context = GetInMemoryContext();
        var service = new MatchService(context);
        var dto = new CreateMatchDto(
            DateTime.UtcNow, 
            DateTime.UtcNow.AddHours(1), 
            null, 
            new List<MatchParticipantDto> { new(999, 1, false) });

        var (result, match) = await service.CreateMatchAsync(dto);

        Assert.Equal(MatchResult.PlayerNotFound, result);
        Assert.Null(match);
    }
}