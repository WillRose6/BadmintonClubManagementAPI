using BadmintonClubAPI.Controllers;
using BadmintonClubAPI.Models.DTOs.Courts;
using BadmintonClubAPI.Services.CourtBookings;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BadmintonClubAPI.Tests.Controllers;

public class CourtsControllerTests
{
    private readonly Mock<ICourtBookingsService> _mockCourtService = new();

    [Fact]
    public async Task GetAllCourts_ShouldReturnOkWithList()
    {
        var courts = new List<CourtResponseDto>
        {
            new(1, 1, true),
            new(2, 2, false)
        };

        _mockCourtService.Setup(s => s.GetCourtsAsync()).ReturnsAsync(courts);
        var controller = new CourtsController(_mockCourtService.Object);

        var result = await controller.GetAllCourts();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedCourts = Assert.IsAssignableFrom<IEnumerable<CourtResponseDto>>(okResult.Value);
        Assert.Equal(2, returnedCourts.Count());
    }

    [Fact]
    public async Task GetCourt_WhenExists_ShouldReturnOk()
    {
        var court = new CourtResponseDto(1, 1, true);

        _mockCourtService.Setup(s => s.GetCourtByIdAsync(1)).ReturnsAsync(court);
        var controller = new CourtsController(_mockCourtService.Object);

        var result = await controller.GetCourt(1);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returned = Assert.IsType<CourtResponseDto>(okResult.Value);
        Assert.Equal(1, returned.Id);
    }

    [Fact]
    public async Task GetCourt_WhenDoesNotExist_ShouldReturnNotFound()
    {
        _mockCourtService.Setup(s => s.GetCourtByIdAsync(999)).ReturnsAsync((CourtResponseDto?)null);
        var controller = new CourtsController(_mockCourtService.Object);

        var result = await controller.GetCourt(999);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task CreateCourt_ShouldReturnCreatedAtAction()
    {
        var createDto = new CreateCourtDto(1);
        var courtResponse = new CourtResponseDto(1, 1, false);

        _mockCourtService.Setup(s => s.CreateCourtAsync(createDto)).ReturnsAsync(courtResponse);
        var controller = new CourtsController(_mockCourtService.Object);

        var result = await controller.CreateCourt(createDto);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(CourtsController.GetCourt), createdResult.ActionName);
        Assert.Equal(courtResponse.Id, createdResult.RouteValues?["id"]);
    }

    [Fact]
    public async Task UpdateCourt_WhenExists_ShouldReturnOk()
    {
        var updateDto = new UpdateCourtDto(2, true);
        var updatedResponse = new CourtResponseDto(1, 2, true);

        _mockCourtService.Setup(s => s.UpdateCourtAsync(1, updateDto)).ReturnsAsync(updatedResponse);
        var controller = new CourtsController(_mockCourtService.Object);

        var result = await controller.UpdateCourt(1, updateDto);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returned = Assert.IsType<CourtResponseDto>(okResult.Value);
        Assert.True(returned.IsActive);
        Assert.Equal(2, returned.CourtIndex);
    }

    [Fact]
    public async Task UpdateCourt_WhenDoesNotExist_ShouldReturnNotFound()
    {
        var updateDto = new UpdateCourtDto(2, true);
        _mockCourtService.Setup(s => s.UpdateCourtAsync(999, updateDto)).ReturnsAsync((CourtResponseDto?)null);
        var controller = new CourtsController(_mockCourtService.Object);

        var result = await controller.UpdateCourt(999, updateDto);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task DeleteCourt_WhenExists_ShouldReturnNoContent()
    {
        _mockCourtService.Setup(s => s.DeleteCourtByIdAsync(1)).ReturnsAsync(true);
        var controller = new CourtsController(_mockCourtService.Object);

        var result = await controller.DeleteCourt(1);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteCourt_WhenDoesNotExist_ShouldReturnNotFound()
    {
        _mockCourtService.Setup(s => s.DeleteCourtByIdAsync(999)).ReturnsAsync(false);
        var controller = new CourtsController(_mockCourtService.Object);

        var result = await controller.DeleteCourt(999);

        Assert.IsType<NotFoundObjectResult>(result);
    }
}