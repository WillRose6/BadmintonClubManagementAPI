namespace BadmintonClubAPI.Models.DTOs.Bookings;

public record UpdateBookingDto(int? PlayerId, int? CourtId, DateTime? StartDate, DateTime? EndDate);