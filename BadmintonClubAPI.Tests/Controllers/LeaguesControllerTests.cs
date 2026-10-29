using BadmintonClubAPI.Controllers;
using BadmintonClubAPI.Models.DTOs.Leagues;
using BadmintonClubAPI.Services.Leagues;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BadmintonClubAPI.Tests.Controllers;

public class LeaguesControllerTests
{
    private readonly Mock<ILeagueService> _mockLeagueService = new();

    [Fact]
    public async Task GetLeagueById_WhenExists_ShouldReturnOk()
    {
        var dto = new LeagueResponseDto(1, "Division 1", DateTime.UtcNow, DateTime.UtcNow.AddMonths(1), [], []);
        _mockLeagueService.Setup(s => s.GetLeagueByIdAsync(1)).ReturnsAsync(dto);

        var controller = new LeaguesController(_mockLeagueService.Object);
        var result = await controller.GetLeagueById(1);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returned = Assert.IsType<LeagueResponseDto>(okResult.Value);
        Assert.Equal(1, returned.Id);
    }

    [Fact]
    public async Task AddPlayerToLeague_WhenPlayerNotFound_ShouldReturnNotFound()
    {
        _mockLeagueService.Setup(s => s.AddPlayerToLeague(1, 999)).ReturnsAsync(LeagueResult.PlayerNotFound);

        var controller = new LeaguesController(_mockLeagueService.Object);
        var result = await controller.AddPlayerToLeague(1, 999);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task AddPlayerToLeague_WhenAlreadyInLeague_ShouldReturnConflict()
    {
        _mockLeagueService.Setup(s => s.AddPlayerToLeague(1, 10)).ReturnsAsync(LeagueResult.PlayerAlreadyInLeague);

        var controller = new LeaguesController(_mockLeagueService.Object);
        var result = await controller.AddPlayerToLeague(1, 10);

        Assert.IsType<ConflictObjectResult>(result);
    }
}