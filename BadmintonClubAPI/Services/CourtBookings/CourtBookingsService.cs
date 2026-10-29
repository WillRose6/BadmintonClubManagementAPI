using BadmintonClubAPI.Data;
using BadmintonClubAPI.Models.DTOs.Bookings;
using BadmintonClubAPI.Models.DTOs.Courts;
using BadmintonClubAPI.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BadmintonClubAPI.Services.CourtBookings;

public class CourtBookingsService : ICourtBookingsService
{
    private readonly BadmintonDbContext _context;

    public CourtBookingsService(BadmintonDbContext context)
    {
        _context = context;
    }
    
    // Court operations
    public async Task<IEnumerable<CourtResponseDto>> GetCourtsAsync()
    {
        return await _context.Courts.Select(c => new CourtResponseDto(c.Id, c.CourtIndex, c.IsActive)).ToListAsync();
    }

    public async Task<CourtResponseDto?> GetCourtByIdAsync(int id)
    {
        var court = await _context.Courts.FindAsync(id);
        if (court == null)
        {
            return null;
        }

        return new CourtResponseDto(court.Id, court.CourtIndex, court.IsActive);
    }

    public async Task<CourtResponseDto> CreateCourtAsync(CreateCourtDto courtDto)
    {
        var newCourt = new Court(courtDto.CourtIndex);
        
        _context.Courts.Add(newCourt);
        await _context.SaveChangesAsync();

        return new CourtResponseDto(newCourt.Id, newCourt.CourtIndex, newCourt.IsActive);
    }

    public async Task<CourtResponseDto?> UpdateCourtAsync(int id, UpdateCourtDto courtDto)
    {
        var court = await _context.Courts.FindAsync(id);
        if (court == null)
        {
            return null;
        }

        if (courtDto.Index.HasValue)
        {
            court.CourtIndex = courtDto.Index.Value;
        }

        if (courtDto.IsActive.HasValue)
        {
            court.IsActive = courtDto.IsActive.Value;
        }
        
        await _context.SaveChangesAsync();
        return new CourtResponseDto(court.Id, court.CourtIndex, court.IsActive);
    }

    public async Task<bool> DeleteCourtByIdAsync(int id)
    {
        var court = await _context.Courts.FindAsync(id);
        if (court == null)
        {
            return false;
        }

        _context.Courts.Remove(court);
        await _context.SaveChangesAsync();
        return true;
    }

    // Booking operations
    public async Task<IEnumerable<BookingResponseDto>> GetBookingsAsync()
    {
        return await _context.Bookings.Select(b => new BookingResponseDto(b.Id, b.PlayerId, b.CourtId, b.StartDate, b.EndDate, b.CreatedDate)).ToListAsync();
    }

    public async Task<IEnumerable<BookingResponseDto>> GetBookingsByCourtIdAsync(int id)
    {
        return await _context.Bookings.Where(b => b.CourtId == id).Select(b => new BookingResponseDto(b.Id, b.PlayerId, b.CourtId, b.StartDate, b.EndDate, b.CreatedDate)).ToListAsync();
    }

    public async Task<IEnumerable<BookingResponseDto>> GetBookingsByPlayerIdAsync(int id)
    {
        return await _context.Bookings.Where(b => b.PlayerId == id).Select(b => new BookingResponseDto(b.Id, b.PlayerId, b.CourtId, b.StartDate, b.EndDate, b.CreatedDate)).ToListAsync();
    }

    public async Task<BookingResponseDto?> GetBookingByIdAsync(int id)
    {
        var booking = await _context.Bookings.FindAsync(id);
        if (booking == null)
        {
            return null;
        }
        
        return new BookingResponseDto(booking.Id, booking.PlayerId, booking.CourtId, booking.StartDate, booking.EndDate, booking.CreatedDate);
    }

    public async Task<(CourtBookingResult Result, BookingResponseDto? Booking)> CreateBookingAsync(CreateBookingDto bookingDto)
    {
        var court = await _context.Courts.FindAsync(bookingDto.CourtId);
        if (court == null || !court.IsActive)
        {
            return (CourtBookingResult.CourtNotFoundOrInactive, null);
        }

        var playerExists = await _context.Players.AnyAsync(p => p.Id == bookingDto.PlayerId);
        if (!playerExists)
        {
            return (CourtBookingResult.PlayerNotFound , null);
        }
        
        if (await HasOverlappingBooking(bookingDto.CourtId, bookingDto.StartDate, bookingDto.EndDate))
        {
            return (CourtBookingResult.Overlap, null);
        }

        var booking = new Booking(bookingDto.PlayerId, bookingDto.CourtId, bookingDto.StartDate, bookingDto.EndDate);
        
        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        var bookingResponseDto = new BookingResponseDto(booking.Id, booking.PlayerId, booking.CourtId, booking.StartDate, booking.EndDate, booking.CreatedDate);
        return (CourtBookingResult.Success, bookingResponseDto);
    }
    
    public async Task<(CourtBookingResult Result, BookingResponseDto? Booking)> UpdateBookingAsync(int id, UpdateBookingDto bookingDto)
    {
        var booking = await _context.Bookings.FindAsync(id);
        if (booking == null)
        {
            return (CourtBookingResult.BookingNotFound, null);
        }

        int targetCourtId = bookingDto.CourtId ?? booking.CourtId;
        DateTime targetStartDate = bookingDto.StartDate ?? booking.StartDate;
        DateTime targetEndDate = bookingDto.EndDate ?? booking.EndDate;

        var court = await _context.Courts.FindAsync(targetCourtId);
        if (court == null || !court.IsActive)
        {
            return (CourtBookingResult.CourtNotFoundOrInactive, null);
        }

        if (await HasOverlappingBooking(targetCourtId, targetStartDate, targetEndDate, id))
        {
            return (CourtBookingResult.Overlap, null);
        }

        booking.CourtId = targetCourtId;
        booking.StartDate = targetStartDate;
        booking.EndDate = targetEndDate;

        await _context.SaveChangesAsync();

        var responseDto = new BookingResponseDto(booking.Id, booking.PlayerId, booking.CourtId, booking.StartDate, booking.EndDate, booking.CreatedDate);
        return new (CourtBookingResult.Success, responseDto);
    }

    public async Task<bool> CancelBookingAsync(int id)
    {
        var booking = await _context.Bookings.FindAsync(id);
        if (booking == null)
        {
            return false;
        }

        _context.Bookings.Remove(booking);
        await _context.SaveChangesAsync();
        return true;
    }

    private async Task<bool> HasOverlappingBooking(int targetCourtId, DateTime targetStartDate, DateTime targetEndDate, int? excludeBookingId = null)
    {
        return await _context.Bookings.AnyAsync(b => (!excludeBookingId.HasValue || b.Id != excludeBookingId.Value) && b.CourtId == targetCourtId && targetStartDate < b.EndDate && targetEndDate > b.StartDate);
    }
}