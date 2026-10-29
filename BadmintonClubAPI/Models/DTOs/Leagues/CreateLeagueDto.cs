namespace BadmintonClubAPI.Models.DTOs.Leagues;

public record CreateLeagueDto(string Name, DateTime StartDate, DateTime EndDate);