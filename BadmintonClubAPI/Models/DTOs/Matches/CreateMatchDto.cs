namespace BadmintonClubAPI.Models.DTOs.Matches;

public record CreateMatchDto(DateTime StartDate, DateTime EndDate, int? LeagueId, List<MatchParticipantDto> Participants);