using BadmintonClubAPI.Models.DTOs;
using BadmintonClubAPI.Models.DTOs.Players;

namespace BadmintonClubAPI.Services.Players;

public interface IPlayerService
{
    Task<IEnumerable<PlayerResponseDto>> GetAllPlayersAsync();
    Task<PlayerResponseDto?> GetPlayerByIdAsync(int id);
    Task<PlayerResponseDto> CreatePlayerAsync(CreatePlayerDto createDto);
    Task<PlayerResponseDto?> UpdatePlayerByIdAsync(int id, UpdatePlayerDto updatePlayerDto);
    Task<bool> DeletePlayerAsync(int id);
}