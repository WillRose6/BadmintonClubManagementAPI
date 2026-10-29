namespace BadmintonClubAPI.Services.CourtBookings;

public enum CourtBookingResult
{
    Success,
    BookingNotFound,
    CourtNotFoundOrInactive,
    PlayerNotFound,
    Overlap
}