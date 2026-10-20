namespace BadmintonClubAPI.Models.DTOs.Players;

public record PlayerResponseDto(int Id, string FirstName, string Surname, string EmailAddress, DateTime DateOfBirth);