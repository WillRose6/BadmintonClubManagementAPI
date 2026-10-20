namespace BadmintonClubAPI.Models.DTOs.Bookings;

public record BookingResponseDto(int Id, int PlayerId, int CourtId, DateTime StartDate, DateTime EndDate, DateTime CreatedDate);