using BadmintonClubAPI.Models.DTOs.Leagues;
using Microsoft.AspNetCore.Mvc;

namespace BadmintonClubAPI.Services.Leagues;

public interface ILeagueService
{
    Task<IEnumerable<LeagueResponseDto>> GetAllLeaguesAsync();
    Task<LeagueResponseDto?> GetLeagueByIdAsync(int id);
    Task<LeagueResponseDto> CreateLeague(CreateLeagueDto createLeagueDto);
    Task<LeagueResult> AddPlayerToLeague(int leagueId, int playerId);
    Task<LeagueResult> RemovePlayerFromLeague(int leagueId, int playerId);
    Task<bool> DeleteLeagueById(int id);
}