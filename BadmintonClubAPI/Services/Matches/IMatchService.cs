using BadmintonClubAPI.Models.DTOs.Matches;

namespace BadmintonClubAPI.Services.Matches;

public interface IMatchService
{
    Task<IEnumerable<MatchResponseDto>> GetAllMatchesAsync();
    Task<MatchResponseDto?> GetMatchByIdAsync(int id);
    Task<IEnumerable<MatchResponseDto>> GetMatchesByLeagueIdAsync(int leagueId);
    Task<(MatchResult Result, MatchResponseDto? Match)> CreateMatchAsync(CreateMatchDto createMatchDto);
    Task<bool> DeleteMatchAsync(int id);
}