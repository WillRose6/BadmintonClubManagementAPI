namespace BadmintonClubAPI.Models.DTOs.Players;

public record UpdatePlayerDto(string? FirstName, string? Surname, string? EmailAddress, DateTime? DateOfBirth);