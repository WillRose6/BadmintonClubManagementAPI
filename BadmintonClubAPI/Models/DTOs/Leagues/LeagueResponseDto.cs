namespace BadmintonClubAPI.Models.DTOs.Leagues;

public record LeagueResponseDto(int Id, string Name, DateTime StartDate, DateTime EndDate, IEnumerable<int> PlayerIds, IEnumerable<int> MatchIds);