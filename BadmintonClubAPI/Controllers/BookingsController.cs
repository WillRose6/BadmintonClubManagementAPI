using BadmintonClubAPI.Models.DTOs.Bookings;
using BadmintonClubAPI.Services.CourtBookings;
using Microsoft.AspNetCore.Authorization;
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

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateBooking([FromBody] CreateBookingDto createBookingDto)
    {
        var (result, booking) = await _courtBookingService.CreateBookingAsync(createBookingDto);

        return result switch
        {
            CourtBookingResult.Success => CreatedAtAction(nameof(GetBookingById), new { id = booking!.Id }, booking),
            CourtBookingResult.CourtNotFoundOrInactive => NotFound(new { message = $"Court with ID { createBookingDto.CourtId } not found or was inactive." }),
            CourtBookingResult.PlayerNotFound => NotFound(new { message = $"Player with ID { createBookingDto.PlayerId } not found." }),
            CourtBookingResult.Overlap => Conflict(new { message = $"Booking times overlap with existing booking."}),
            _ => BadRequest()
        };
    }

    [Authorize]
    [HttpPatch("{id:int}")]
    public async Task<IActionResult> UpdateBooking(int id, [FromBody] UpdateBookingDto updateBookingDto)
    {
        var (result, booking) = await _courtBookingService.UpdateBookingAsync(id, updateBookingDto);

        return result switch
        {
            CourtBookingResult.Success => Ok(booking),
            CourtBookingResult.BookingNotFound => NotFound(new { message = $"Booking with ID { id } not found." }),
            CourtBookingResult.CourtNotFoundOrInactive => BadRequest(new { message = $"Court with id { updateBookingDto.CourtId } does not exist or is inactive." }),
            CourtBookingResult.Overlap => Conflict(new { message = "Booking times overlap with existing booking." }),
            _ => BadRequest()
        };
    }

    [Authorize]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteBooking(int id)
    {
        bool success = await _courtBookingService.CancelBookingAsync(id);
        return success ? NoContent() : NotFound(new { message = $"No booking could be found with id {id}"});
    }
}