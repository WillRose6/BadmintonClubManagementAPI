using BadmintonClubAPI.Controllers;
using BadmintonClubAPI.Models.DTOs.Players;
using BadmintonClubAPI.Services.Players;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BadmintonClubAPI.Tests.Controllers;

public class PlayersControllerTests
{
    private readonly Mock<IPlayerService> _mockPlayerService = new();

    [Fact]
    public async Task GetAllPlayers_ShouldReturnOkWithList()
    {
        var players = new List<PlayerResponseDto>
        {
            new(1, "Daniella", "Smith", "Daniella.Smith@gmail.com", new DateTime(2000, 5, 20)),
            new(2, "Alex", "Price", "Alex.Price@gmail.com", new DateTime(1995, 2, 18))
        };

        _mockPlayerService.Setup(s => s.GetAllPlayersAsync()).ReturnsAsync(players);
        var controller = new PlayersController(_mockPlayerService.Object);

        var result = await controller.GetAllPlayers();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returned = Assert.IsAssignableFrom<IEnumerable<PlayerResponseDto>>(okResult.Value);
        Assert.Equal(2, returned.Count());
    }

    [Fact]
    public async Task GetPlayerById_WhenExists_ShouldReturnOk()
    {
        var player = new PlayerResponseDto(1, "Daniella", "Smith", "Daniella.Smith@gmail.com", new DateTime(2000, 5, 20));

        _mockPlayerService.Setup(s => s.GetPlayerByIdAsync(1)).ReturnsAsync(player);
        var controller = new PlayersController(_mockPlayerService.Object);

        var result = await controller.GetPlayerById(1);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returned = Assert.IsType<PlayerResponseDto>(okResult.Value);
        Assert.Equal(1, returned.Id);
        Assert.Equal("Daniella", returned.FirstName);
    }

    [Fact]
    public async Task GetPlayerById_WhenDoesNotExist_ShouldReturnNotFound()
    {
        _mockPlayerService.Setup(s => s.GetPlayerByIdAsync(999)).ReturnsAsync((PlayerResponseDto?)null);
        var controller = new PlayersController(_mockPlayerService.Object);

        var result = await controller.GetPlayerById(999);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task CreatePlayer_ShouldReturnCreatedAtAction()
    {
        var createDto = new CreatePlayerDto("Daniella", "Smith", new DateTime(2000, 5, 20), "Daniella.Smith@gmail.com", "Password123!");
        var playerResponse = new PlayerResponseDto(1, createDto.FirstName, createDto.Surname, createDto.EmailAddress, createDto.DateOfBirth);

        _mockPlayerService.Setup(s => s.CreatePlayerAsync(createDto)).ReturnsAsync(playerResponse);
        var controller = new PlayersController(_mockPlayerService.Object);

        var result = await controller.CreatePlayer(createDto);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(PlayersController.GetPlayerById), createdResult.ActionName);
        Assert.Equal(playerResponse.Id, createdResult.RouteValues?["id"]);

        var returned = Assert.IsType<PlayerResponseDto>(createdResult.Value);
        Assert.Equal(playerResponse.Id, returned.Id);
    }

    [Fact]
    public async Task UpdatePlayer_WhenExists_ShouldReturnOk()
    {
        var updateDto = new UpdatePlayerDto("Daniella-Updated", null, null, null);
        var updatedResponse = new PlayerResponseDto(1, "Daniella-Updated", "Smith", "Daniella.Smith@gmail.com", new DateTime(2000, 5, 20));

        _mockPlayerService.Setup(s => s.UpdatePlayerByIdAsync(1, updateDto)).ReturnsAsync(updatedResponse);
        var controller = new PlayersController(_mockPlayerService.Object);

        var result = await controller.UpdatePlayer(1, updateDto);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returned = Assert.IsType<PlayerResponseDto>(okResult.Value);
        Assert.Equal("Daniella-Updated", returned.FirstName);
    }

    [Fact]
    public async Task UpdatePlayer_WhenDoesNotExist_ShouldReturnNotFound()
    {
        var updateDto = new UpdatePlayerDto("Daniella-Updated", null, null, null);

        _mockPlayerService.Setup(s => s.UpdatePlayerByIdAsync(999, updateDto)).ReturnsAsync((PlayerResponseDto?)null);
        var controller = new PlayersController(_mockPlayerService.Object);

        var result = await controller.UpdatePlayer(999, updateDto);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task DeletePlayer_WhenExists_ShouldReturnNoContent()
    {
        _mockPlayerService.Setup(s => s.DeletePlayerAsync(1)).ReturnsAsync(true);
        var controller = new PlayersController(_mockPlayerService.Object);

        var result = await controller.DeletePlayer(1);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeletePlayer_WhenDoesNotExist_ShouldReturnNotFound()
    {
        _mockPlayerService.Setup(s => s.DeletePlayerAsync(999)).ReturnsAsync(false);
        var controller = new PlayersController(_mockPlayerService.Object);

        var result = await controller.DeletePlayer(999);

        Assert.IsType<NotFoundObjectResult>(result);
    }
}