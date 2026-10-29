namespace BadmintonClubAPI.Models.DTOs.Matches;

public record MatchResponseDto(int Id, DateTime StartDate, DateTime EndDate, int? LeagueId, List<MatchParticipantDto> Participants);