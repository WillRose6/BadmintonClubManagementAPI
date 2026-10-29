using BadmintonClubAPI.Models.DTOs.Leagues;
using BadmintonClubAPI.Services.Leagues;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BadmintonClubAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LeaguesController : ControllerBase
{
    private readonly ILeagueService _leagueService;
    
    public LeaguesController(ILeagueService leagueService)
    {
        _leagueService = leagueService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllLeagues()
    {
        var leagues = await _leagueService.GetAllLeaguesAsync();
        return Ok(leagues);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetLeagueById(int id)
    {
        var league = await _leagueService.GetLeagueByIdAsync(id);
        return league == null 
            ? NotFound(new { message = $"Unable to find league with id {id}" }) 
            : Ok(league); 
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateLeague([FromBody] CreateLeagueDto createLeagueDto)
    {
        var createdLeague = await _leagueService.CreateLeague(createLeagueDto);
        return CreatedAtAction(nameof(GetLeagueById), new { id = createdLeague.Id }, createdLeague);
    }

    [Authorize]
    [HttpPost("{leagueId:int}/players/{playerId:int}")]
    public async Task<IActionResult> AddPlayerToLeague(int leagueId, int playerId)
    {
        LeagueResult result = await _leagueService.AddPlayerToLeague(leagueId, playerId);

        return result switch
        {
            LeagueResult.Success => Ok(new { message = $"Successfully added player {playerId} to league {leagueId}" }),
            LeagueResult.PlayerNotFound => NotFound(new { message = $"Unable to find player with id {playerId}" }),
            LeagueResult.LeagueNotFound => NotFound(new { message = $"Unable to find league with id {leagueId}" }),
            LeagueResult.PlayerAlreadyInLeague => Conflict(new { message = $"League {leagueId} already contains player {playerId}" }),
            _ => BadRequest()
        };
    }

    [Authorize]
    [HttpDelete("{leagueId:int}/players/{playerId:int}")]
    public async Task<IActionResult> RemovePlayerFromLeague(int leagueId, int playerId)
    {
        LeagueResult result = await _leagueService.RemovePlayerFromLeague(leagueId, playerId);

        return result switch
        {
            LeagueResult.Success => NoContent(),
            LeagueResult.PlayerNotFound => NotFound(new { message = $"Unable to find player with id {playerId}" }),
            LeagueResult.LeagueNotFound => NotFound(new { message = $"Unable to find league with id {leagueId}" }),
            LeagueResult.PlayerNotInLeague => NotFound(new { message = $"Could not find player {playerId} in league {leagueId} to remove them" }),
            _ => BadRequest()
        };
    }

    [Authorize]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteLeagueById(int id)
    {
        bool bSuccess = await _leagueService.DeleteLeagueById(id);
        return !bSuccess 
            ? NotFound(new { message = $"Unable to find league with id {id}" }) 
            : NoContent();
    }
}