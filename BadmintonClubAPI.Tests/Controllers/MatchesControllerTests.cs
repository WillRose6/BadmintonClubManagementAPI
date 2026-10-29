using BadmintonClubAPI.Controllers;
using BadmintonClubAPI.Models.DTOs.Matches;
using BadmintonClubAPI.Services.Matches;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BadmintonClubAPI.Tests.Controllers;

public class MatchesControllerTests
{
    private readonly Mock<IMatchService> _mockMatchService = new();

    [Fact]
    public async Task CreateMatch_WhenSuccessful_ShouldReturnCreatedAtAction()
    {
        var dto = new CreateMatchDto(DateTime.UtcNow, DateTime.UtcNow.AddHours(1), null, []);
        var response = new MatchResponseDto(1, dto.StartDate, dto.EndDate, null, []);

        _mockMatchService.Setup(s => s.CreateMatchAsync(dto)).ReturnsAsync((MatchResult.Success, response));

        var controller = new MatchesController(_mockMatchService.Object);
        var result = await controller.CreateMatch(dto);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(MatchesController.GetMatchById), createdResult.ActionName);
    }

    [Fact]
    public async Task CreateMatch_WhenPlayerNotFound_ShouldReturnNotFound()
    {
        var dto = new CreateMatchDto(DateTime.UtcNow, DateTime.UtcNow.AddHours(1), null, []);

        _mockMatchService.Setup(s => s.CreateMatchAsync(dto)).ReturnsAsync((MatchResult.PlayerNotFound, null));

        var controller = new MatchesController(_mockMatchService.Object);
        var result = await controller.CreateMatch(dto);

        Assert.IsType<NotFoundObjectResult>(result);
    }
}