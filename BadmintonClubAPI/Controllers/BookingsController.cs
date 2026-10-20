using BadmintonClubAPI.Models.DTOs.Bookings;
using BadmintonClubAPI.Services.CourtBookings;
using Microsoft.AspNetCore.Mvc;

namespace BadmintonClubAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingsController : ControllerBase
{
    private readonly ICourtBookingsService _courtBookingService;

    public BookingsController(ICourtBookingsService courtBookingService)
    {
        _courtBookingService = courtBookingService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllBookings()
    {
        var bookings = await _courtBookingService.GetBookingsAsync();
        return Ok(bookings);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetBookingById(int id)
    {
        var booking = await _courtBookingService.GetBookingByIdAsync(id);
        return booking == null ? NotFound(new { message = $"Booking with ID {id} not found." }) : Ok(booking);
    }

    [HttpGet("player/{playerId:int}")]
    public async Task<IActionResult> GetBookingByPlayerId(int playerId)
    {
        var bookings = await _courtBookingService.GetBookingsByPlayerIdAsync(playerId);
        return Ok(bookings);
    }

    [HttpGet("court/{courtId:int}")]
    public async Task<IActionResult> GetBookingByCourtId(int courtId)
    {
        var bookings = await _courtBookingService.GetBookingsByCourtIdAsync(courtId);
        return Ok(bookings);
    }

    [HttpPost]
    public async Task<IActionResult> CreateBooking([FromBody] CreateBookingDto createBookingDto)
    {
        var booking = await _courtBookingService.CreateBookingAsync(createBookingDto);
        if (booking == null)
        {
            return BadRequest(new { message = $"Could not create booking. It may be inactive or already taken"});
        }
        
        return CreatedAtAction(nameof(GetBookingById), new { id = booking.Id }, booking);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteBooking(int id)
    {
        bool success = await _courtBookingService.CancelBookingAsync(id);
        return success ? NoContent() : NotFound(new { message = $"No booking could be found with id {id}"});
    }
}