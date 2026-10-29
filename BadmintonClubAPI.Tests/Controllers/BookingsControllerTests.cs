using BadmintonClubAPI.Controllers;
using BadmintonClubAPI.Models.DTOs.Bookings;
using BadmintonClubAPI.Services.CourtBookings;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BadmintonClubAPI.Tests.Controllers;

public class BookingsControllerTests
{
    private readonly Mock<ICourtBookingsService> _mockBookingService = new();

    [Fact]
    public async Task GetAllBookings_ShouldReturnOkWithList()
    {
        var bookings = new List<BookingResponseDto>
        {
            new(1, 10, 2, DateTime.UtcNow, DateTime.UtcNow.AddHours(1), DateTime.UtcNow)
        };

        _mockBookingService.Setup(s => s.GetBookingsAsync()).ReturnsAsync(bookings);
        var controller = new BookingsController(_mockBookingService.Object);

        var result = await controller.GetAllBookings();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedBookings = Assert.IsAssignableFrom<IEnumerable<BookingResponseDto>>(okResult.Value);
        Assert.Single(returnedBookings);
    }

    [Fact]
    public async Task GetBookingById_WhenExists_ShouldReturnOk()
    {
        var booking = new BookingResponseDto(1, 10, 2, DateTime.UtcNow, DateTime.UtcNow.AddHours(1), DateTime.UtcNow);

        _mockBookingService.Setup(s => s.GetBookingByIdAsync(1)).ReturnsAsync(booking);
        var controller = new BookingsController(_mockBookingService.Object);

        var result = await controller.GetBookingById(1);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returned = Assert.IsType<BookingResponseDto>(okResult.Value);
        Assert.Equal(1, returned.Id);
    }

    [Fact]
    public async Task GetBookingById_WhenDoesNotExist_ShouldReturnNotFound()
    {
        _mockBookingService.Setup(s => s.GetBookingByIdAsync(999)).ReturnsAsync((BookingResponseDto?)null);
        var controller = new BookingsController(_mockBookingService.Object);

        var result = await controller.GetBookingById(999);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task CreateBooking_WhenSuccessful_ShouldReturnCreatedAtAction()
    {
        var createDto = new CreateBookingDto(10, 2, DateTime.UtcNow, DateTime.UtcNow.AddHours(1));
        var bookingResponse = new BookingResponseDto(1, 10, 2, createDto.StartDate, createDto.EndDate, DateTime.UtcNow);

        _mockBookingService.Setup(s => s.CreateBookingAsync(createDto))
            .ReturnsAsync((CourtBookingResult.Success, bookingResponse));
        var controller = new BookingsController(_mockBookingService.Object);

        var result = await controller.CreateBooking(createDto);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(BookingsController.GetBookingById), createdResult.ActionName);
        Assert.Equal(bookingResponse.Id, createdResult.RouteValues?["id"]);
    }

    [Fact]
    public async Task CreateBooking_WhenOverlap_ShouldReturnConflict()
    {
        var createDto = new CreateBookingDto(10, 2, DateTime.UtcNow, DateTime.UtcNow.AddHours(1));

        _mockBookingService.Setup(s => s.CreateBookingAsync(createDto))
            .ReturnsAsync((CourtBookingResult.Overlap, null));
        var controller = new BookingsController(_mockBookingService.Object);

        var result = await controller.CreateBooking(createDto);

        Assert.IsType<ConflictObjectResult>(result);
    }

    [Fact]
    public async Task DeleteBooking_WhenExists_ShouldReturnNoContent()
    {
        _mockBookingService.Setup(s => s.CancelBookingAsync(1)).ReturnsAsync(true);
        var controller = new BookingsController(_mockBookingService.Object);

        var result = await controller.DeleteBooking(1);
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteBooking_WhenDoesNotExist_ShouldReturnNotFound()
    {
        _mockBookingService.Setup(s => s.CancelBookingAsync(999)).ReturnsAsync(false);
        var controller = new BookingsController(_mockBookingService.Object);

        var result = await controller.DeleteBooking(999);
        Assert.IsType<NotFoundObjectResult>(result);
    }
}