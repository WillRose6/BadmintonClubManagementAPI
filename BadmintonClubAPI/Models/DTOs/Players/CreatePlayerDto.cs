namespace BadmintonClubAPI.Models.DTOs.Players;

public record CreatePlayerDto(string FirstName, string Surname, DateTime DateOfBirth, string EmailAddress, string Password);