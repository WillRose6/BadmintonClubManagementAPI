using BadmintonClubAPI.Data;
using BadmintonClubAPI.Models.DTOs.Bookings;
using BadmintonClubAPI.Models.DTOs.Courts;
using BadmintonClubAPI.Models.Entities;
using BadmintonClubAPI.Services.CourtBookings;
using Microsoft.EntityFrameworkCore;

namespace BadmintonClubAPI.Tests.Services;

public class CourtBookingsServiceTests
{
    private BadmintonDbContext GetInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<BadmintonDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new BadmintonDbContext(options);
    }

    [Fact]
    public async Task GetCourtsAsync_ShouldReturnAllCourts()
    {
        await using var context = GetInMemoryContext();
        var court1 = new Court(1) { IsActive = true };
        var court2 = new Court(2) { IsActive = false };

        context.Courts.AddRange(court1, court2);
        await context.SaveChangesAsync();

        var service = new CourtBookingsService(context);
        var result = (await service.GetCourtsAsync()).ToList();

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Contains(result, c => c.CourtIndex == court1.CourtIndex && c.IsActive == court1.IsActive);
        Assert.Contains(result, c => c.CourtIndex == court2.CourtIndex && c.IsActive == court2.IsActive);
    }

    [Fact]
    public async Task GetCourtByIdAsync_WhenCourtExists_ShouldReturnCourtDto()
    {
        await using var context = GetInMemoryContext();
        var court = new Court(1) { IsActive = true };

        context.Courts.Add(court);
        await context.SaveChangesAsync();

        var service = new CourtBookingsService(context);
        var result = await service.GetCourtByIdAsync(court.Id);

        Assert.NotNull(result);
        Assert.Equal(court.Id, result.Id);
        Assert.Equal(court.CourtIndex, result.CourtIndex);
        Assert.True(result.IsActive);
    }

    [Fact]
    public async Task GetCourtByIdAsync_WhenCourtDoesNotExist_ShouldReturnNull()
    {
        await using var context = GetInMemoryContext();
        var service = new CourtBookingsService(context);

        var result = await service.GetCourtByIdAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateCourtAsync_ShouldAddCourtToDatabase()
    {
        await using var context = GetInMemoryContext();
        var service = new CourtBookingsService(context);
        var createDto = new CreateCourtDto(CourtIndex: 3);

        var result = await service.CreateCourtAsync(createDto);

        Assert.NotNull(result);
        Assert.Equal(createDto.CourtIndex, result.CourtIndex);
        Assert.False(result.IsActive);

        var courtInDb = await context.Courts.FindAsync(result.Id);
        Assert.NotNull(courtInDb);
        Assert.Equal(createDto.CourtIndex, courtInDb.CourtIndex);
    }

    [Fact]
    public async Task UpdateCourtAsync_WhenCourtExists_ShouldUpdateProperties()
    {
        await using var context = GetInMemoryContext();
        var court = new Court(1) { IsActive = false };
        context.Courts.Add(court);
        await context.SaveChangesAsync();

        var service = new CourtBookingsService(context);
        var updateDto = new UpdateCourtDto(Index: 5, IsActive: true);

        var result = await service.UpdateCourtAsync(court.Id, updateDto);

        Assert.NotNull(result);
        Assert.Equal(updateDto.Index!.Value, result.CourtIndex);
        Assert.True(result.IsActive);

        var updatedInDb = await context.Courts.FindAsync(court.Id);
        Assert.NotNull(updatedInDb);
        Assert.Equal(5, updatedInDb.CourtIndex);
        Assert.True(updatedInDb.IsActive);
    }

    [Fact]
    public async Task UpdateCourtAsync_WhenCourtDoesNotExist_ShouldReturnNull()
    {
        await using var context = GetInMemoryContext();
        var service = new CourtBookingsService(context);

        var result = await service.UpdateCourtAsync(999, new UpdateCourtDto(2, true));

        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteCourtByIdAsync_WhenCourtExists_ShouldRemoveAndReturnTrue()
    {
        await using var context = GetInMemoryContext();
        var court = new Court(1);
        context.Courts.Add(court);
        await context.SaveChangesAsync();

        var service = new CourtBookingsService(context);
        var result = await service.DeleteCourtByIdAsync(court.Id);

        Assert.True(result);
        var deletedCourt = await context.Courts.FindAsync(court.Id);
        Assert.Null(deletedCourt);
    }

    [Fact]
    public async Task DeleteCourtByIdAsync_WhenCourtDoesNotExist_ShouldReturnFalse()
    {
        await using var context = GetInMemoryContext();
        var service = new CourtBookingsService(context);

        var result = await service.DeleteCourtByIdAsync(999);

        Assert.False(result);
    }

    [Fact]
    public async Task CreateBookingAsync_WhenValid_ShouldSaveBooking()
    {
        await using var context = GetInMemoryContext();
        var court = new Court(1) { IsActive = true };
        var player = new Player("Daniella", "Smith", "Daniella.Smith@gmail.com", new DateTime(2000, 5, 20));

        context.Courts.Add(court);
        context.Players.Add(player);
        await context.SaveChangesAsync();

        var service = new CourtBookingsService(context);
        var startTime = DateTime.UtcNow.AddDays(1);
        var endTime = startTime.AddHours(1);

        var createDto = new CreateBookingDto(player.Id, court.Id, startTime, endTime);
        var (result, booking) = await service.CreateBookingAsync(createDto);

        Assert.Equal(CourtBookingResult.Success, result);
        Assert.NotNull(booking);
        Assert.Equal(player.Id, booking.PlayerId);
        Assert.Equal(court.Id, booking.CourtId);

        var bookingInDb = await context.Bookings.FindAsync(booking.Id);
        Assert.NotNull(bookingInDb);
    }

    [Fact]
    public async Task CreateBookingAsync_WhenCourtIsInactiveOrMissing_ShouldReturnCourtNotFoundOrInactive()
    {
        await using var context = GetInMemoryContext();
        var inactiveCourt = new Court(1) { IsActive = false };
        var player = new Player("Daniella", "Smith", "Daniella.Smith@gmail.com", new DateTime(2000, 5, 20));

        context.Courts.Add(inactiveCourt);
        context.Players.Add(player);
        await context.SaveChangesAsync();

        var service = new CourtBookingsService(context);
        var createDto = new CreateBookingDto(player.Id, inactiveCourt.Id, DateTime.UtcNow, DateTime.UtcNow.AddHours(1));

        var (result, booking) = await service.CreateBookingAsync(createDto);

        Assert.Equal(CourtBookingResult.CourtNotFoundOrInactive, result);
        Assert.Null(booking);
    }

    [Fact]
    public async Task CreateBookingAsync_WhenPlayerDoesNotExist_ShouldReturnPlayerNotFound()
    {
        await using var context = GetInMemoryContext();
        var court = new Court(1) { IsActive = true };
        context.Courts.Add(court);
        await context.SaveChangesAsync();

        var service = new CourtBookingsService(context);
        var createDto = new CreateBookingDto(PlayerId: 999, court.Id, DateTime.UtcNow, DateTime.UtcNow.AddHours(1));

        var (result, booking) = await service.CreateBookingAsync(createDto);

        Assert.Equal(CourtBookingResult.PlayerNotFound, result);
        Assert.Null(booking);
    }

    [Theory]
    [InlineData(0, 60)]
    [InlineData(-15, 30)]
    [InlineData(30, 90)]
    [InlineData(-15, 75)]
    public async Task CreateBookingAsync_WhenTimeSlotOverlaps_ShouldReturnOverlap(int startOffsetMinutes,
        int endOffsetMinutes)
    {
        await using var context = GetInMemoryContext();
        var court = new Court(1) { IsActive = true };
        var player = new Player("Daniella", "Smith", "Daniella.Smith@gmail.com", new DateTime(2000, 5, 20));

        context.Courts.Add(court);
        context.Players.Add(player);
        await context.SaveChangesAsync();

        var baseTime = DateTime.UtcNow.Date.AddHours(14);
        var existingBooking = new Booking(player.Id, court.Id, baseTime, baseTime.AddMinutes(60));

        context.Bookings.Add(existingBooking);
        await context.SaveChangesAsync();

        var service = new CourtBookingsService(context);
        var newBookingDto = new CreateBookingDto(player.Id, court.Id, baseTime.AddMinutes(startOffsetMinutes),
            baseTime.AddMinutes(endOffsetMinutes));
        var (result, booking) = await service.CreateBookingAsync(newBookingDto);

        Assert.Equal(CourtBookingResult.Overlap, result);
        Assert.Null(booking);
    }

    [Fact]
    public async Task UpdateBookingAsync_WhenValid_ShouldUpdateAndReturnSuccess()
    {
        await using var context = GetInMemoryContext();
        var court1 = new Court(1) { IsActive = true };
        var court2 = new Court(2) { IsActive = true };
        var player = new Player("Daniella", "Smith", "Daniella.Smith@gmail.com", new DateTime(2000, 5, 20));
        var booking = new Booking(player.Id, court1.Id, DateTime.UtcNow.AddDays(1),
            DateTime.UtcNow.AddDays(1).AddHours(1));

        context.Courts.AddRange(court1, court2);
        context.Players.Add(player);
        context.Bookings.Add(booking);
        await context.SaveChangesAsync();

        var service = new CourtBookingsService(context);
        var updateDto = new UpdateBookingDto(null, court2.Id, null, null);

        var (result, updatedBooking) = await service.UpdateBookingAsync(booking.Id, updateDto);

        Assert.Equal(CourtBookingResult.Success, result);
        Assert.NotNull(updatedBooking);
        Assert.Equal(court2.Id, updatedBooking.CourtId);
    }
    
    [Fact]
    public async Task UpdateBookingAsync_WhenTimesOverlapWithAnotherBooking_ShouldReturnOverlap()
    {
        await using var context = GetInMemoryContext();
        var court = new Court(1) { Id = 1, IsActive = true };
        var player = new Player("Daniella", "Smith", "Daniella.Smith@gmail.com", new DateTime(2000, 5, 20)) { Id = 1 };

        var baseTime = DateTime.UtcNow.Date.AddHours(14);
        var booking1 = new Booking(player.Id, court.Id, baseTime, baseTime.AddHours(1)) { Id = 1 };
        var booking2 = new Booking(player.Id, court.Id, baseTime.AddHours(2), baseTime.AddHours(3)) { Id = 2 };

        context.Courts.Add(court);
        context.Players.Add(player);
        context.Bookings.AddRange(booking1, booking2);
        await context.SaveChangesAsync();

        var service = new CourtBookingsService(context);

        var updateDto = new UpdateBookingDto(null, court.Id, baseTime.AddMinutes(30), baseTime.AddHours(2.5));
        var (result, booking) = await service.UpdateBookingAsync(booking2.Id, updateDto);

        Assert.Equal(CourtBookingResult.Overlap, result);
        Assert.Null(booking);
    }

    [Fact]
    public async Task CancelBookingAsync_WhenBookingExists_ShouldRemoveAndReturnTrue()
    {
        await using var context = GetInMemoryContext();
        var court = new Court(1) { IsActive = true };
        var player = new Player("Daniella", "Smith", "Daniella.Smith@gmail.com", new DateTime(2000, 5, 20));
        var booking = new Booking(player.Id, court.Id, DateTime.UtcNow, DateTime.UtcNow.AddHours(1));

        context.Courts.Add(court);
        context.Players.Add(player);
        context.Bookings.Add(booking);
        await context.SaveChangesAsync();

        var service = new CourtBookingsService(context);
        var result = await service.CancelBookingAsync(booking.Id);

        Assert.True(result);
        var deletedBooking = await context.Bookings.FindAsync(booking.Id);
        Assert.Null(deletedBooking);
    }

    [Fact]
    public async Task CancelBookingAsync_WhenBookingDoesNotExist_ShouldReturnFalse()
    {
        await using var context = GetInMemoryContext();
        var service = new CourtBookingsService(context);

        var result = await service.CancelBookingAsync(999);

        Assert.False(result);
    }
}