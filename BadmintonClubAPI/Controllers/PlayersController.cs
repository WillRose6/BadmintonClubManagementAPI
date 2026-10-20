using BadmintonClubAPI.Models.DTOs.Players;
using BadmintonClubAPI.Services.Players;
using Microsoft.AspNetCore.Mvc;

namespace BadmintonClubAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlayersController : ControllerBase
{
    private readonly IPlayerService _playerService;
    
    public PlayersController(IPlayerService playerService)
    {
        _playerService = playerService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllPlayers()
    {
        var players = await _playerService.GetAllPlayersAsync();
        return Ok(players);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetPlayerById(int id)
    {
        var player = await _playerService.GetPlayerByIdAsync(id);
        return player == null ? NotFound(new { Message = $"Cannot find player with ID {id}!"}) : Ok(player);
    }

    [HttpPost]
    public async Task<IActionResult> CreatePlayer([FromBody] CreatePlayerDto createPlayerDto)
    {
        var createdPlayer = await _playerService.CreatePlayerAsync(createPlayerDto);
        return CreatedAtAction(nameof(GetPlayerById), new { id = createdPlayer.Id }, createdPlayer);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdatePlayer(int id, [FromBody] UpdatePlayerDto updatePlayerDto)
    {
        var player = await _playerService.UpdatePlayerByIdAsync(id, updatePlayerDto);
        return player == null ? NotFound(new { Message = $"Cannot find player with ID {id}!" }) : Ok(player);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeletePlayer(int id)
    {
        bool bSuccess = await _playerService.DeletePlayerAsync(id);
        return !bSuccess ? NotFound(new { Message = $"No player found with id {id}!"}) : NoContent();
    }
}