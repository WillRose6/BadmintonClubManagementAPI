using BadmintonClubAPI.Models.DTOs.Courts;
using BadmintonClubAPI.Services.CourtBookings;
using Microsoft.AspNetCore.Mvc;

namespace BadmintonClubAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CourtsController : ControllerBase
{
    private readonly ICourtBookingsService _courtBookingService;
    
    public CourtsController(ICourtBookingsService courtBookingsService)
    {
        _courtBookingService = courtBookingsService;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAllCourts()
    {
        var courts = await _courtBookingService.GetCourtsAsync();
        return Ok(courts);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetCourt(int id)
    {
        var court = await _courtBookingService.GetCourtByIdAsync(id);
        return court == null ? NotFound(new { message = $"Could not find court with id {id}"}) : Ok(court);
    }

    [HttpPost]
    public async Task<IActionResult> CreateCourt([FromBody] CreateCourtDto createCourtDto)
    {
        var createdCourtResponse = await _courtBookingService.CreateCourtAsync(createCourtDto);
        return CreatedAtAction(nameof(GetCourt), new { id = createdCourtResponse.Id },  createdCourtResponse);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateCourt(int id, [FromBody] UpdateCourtDto updateCourtDto)
    {
        var court = await _courtBookingService.UpdateCourtAsync(id, updateCourtDto);
        return court == null ? NotFound( new { message = $"Could not find court with id {id}" }) : Ok(court);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteCourt(int id)
    {
        bool bSuccess = await _courtBookingService.DeleteCourtByIdAsync(id);
        return bSuccess ? NoContent() : NotFound( new { message = $"Could not find court with id {id}" });
    }
}