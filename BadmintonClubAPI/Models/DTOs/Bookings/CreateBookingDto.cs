namespace BadmintonClubAPI.Models.DTOs.Bookings;

public record CreateBookingDto(int PlayerId, int CourtId, DateTime StartDate, DateTime EndDate);