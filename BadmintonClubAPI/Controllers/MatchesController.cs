using BadmintonClubAPI.Models.DTOs.Matches;
using BadmintonClubAPI.Services.Matches;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BadmintonClubAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MatchesController : ControllerBase
{
    private readonly IMatchService _matchService;

    public MatchesController(IMatchService matchService)
    {
        _matchService = matchService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllMatches()
    {
        return Ok(await _matchService.GetAllMatchesAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetMatchById(int id)
    {
        var match = await _matchService.GetMatchByIdAsync(id);
        return match == null ? NotFound(new { message = $"Match with ID {id} not found." }) : Ok(match);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateMatch([FromBody] CreateMatchDto createMatchDto)
    {
        var (result, match) = await _matchService.CreateMatchAsync(createMatchDto);

        return result switch
        {
            MatchResult.Success => CreatedAtAction(nameof(GetMatchById), new { id = match!.Id }, match),
            MatchResult.LeagueNotFound => NotFound(new { message = "Specified league not found." }),
            MatchResult.PlayerNotFound => NotFound(new { message = "One or more specified players were not found." }),
            _ => BadRequest()
        };
    }

    [Authorize]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteMatch(int id)
    {
        bool success = await _matchService.DeleteMatchAsync(id);
        return success ? NoContent() : NotFound(new { message = $"Match with ID {id} not found." });
    }
}