using BadmintonClubAPI.Models.DTOs;
using BadmintonClubAPI.Models.DTOs.Bookings;
using BadmintonClubAPI.Models.DTOs.Courts;
using BadmintonClubAPI.Models.Entities;

namespace BadmintonClubAPI.Services.CourtBookings;

public interface ICourtBookingsService
{
    // Court operations
    Task<IEnumerable<CourtResponseDto>> GetCourtsAsync();
    Task<CourtResponseDto?> GetCourtByIdAsync(int id);
    Task<CourtResponseDto> CreateCourtAsync(CreateCourtDto courtDto);
    Task<CourtResponseDto?> UpdateCourtAsync(int id, UpdateCourtDto courtDto);
    Task<bool> DeleteCourtByIdAsync(int id);
    
    // Booking operations
    Task<IEnumerable<BookingResponseDto>> GetBookingsAsync();
    Task<IEnumerable<BookingResponseDto>> GetBookingsByCourtIdAsync(int id);
    Task<IEnumerable<BookingResponseDto>> GetBookingsByPlayerIdAsync(int id);
    Task<BookingResponseDto?> GetBookingByIdAsync(int id);
    Task<(CourtBookingResult Result, BookingResponseDto? Booking)> CreateBookingAsync(CreateBookingDto bookingDto);
    Task<(CourtBookingResult Result, BookingResponseDto? Booking)> UpdateBookingAsync(int id, UpdateBookingDto updateBookingDto);
    Task<bool> CancelBookingAsync(int id);
}