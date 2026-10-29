namespace BadmintonClubAPI.Models.DTOs.Matches;

public record MatchParticipantDto(int PlayerId, int TeamNumber, bool IsWinner);