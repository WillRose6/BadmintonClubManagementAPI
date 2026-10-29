using BadmintonClubAPI.Models.DTOs.Bookings;
using BadmintonClubAPI.Validators;

namespace BadmintonClubAPI.Tests.Validators;

public class BookingValidatorTests
{
    [Fact]
    public void CreateBookingDtoValidator_WhenValid_ShouldPass()
    {
        var validator = new CreateBookingDtoValidator();
        var startDate = DateTime.UtcNow.AddHours(1);
        var validDto = new CreateBookingDto(1, 1, startDate, startDate.AddHours(1));

        var result = validator.Validate(validDto);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void CreateBookingDtoValidator_WhenEndDateBeforeStartDate_ShouldHaveValidationError()
    {
        var validator = new CreateBookingDtoValidator();
        var startDate = DateTime.UtcNow.AddHours(2);
        var endDate = startDate.AddHours(-1);

        var invalidDto = new CreateBookingDto(1, 1, startDate, endDate);

        var result = validator.Validate(invalidDto);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateBookingDto.EndDate));
    }

    [Fact]
    public void CreateBookingDtoValidator_WhenIdsInvalid_ShouldHaveValidationErrors()
    {
        var validator = new CreateBookingDtoValidator();
        var startDate = DateTime.UtcNow.AddHours(1);
        var invalidDto = new CreateBookingDto(0, -1, startDate, startDate.AddHours(1));

        var result = validator.Validate(invalidDto);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateBookingDto.PlayerId));
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateBookingDto.CourtId));
    }

    [Fact]
    public void UpdateBookingDtoValidator_WhenEndDateBeforeStartDate_ShouldHaveValidationError()
    {
        var validator = new UpdateBookingDtoValidator();
        var startDate = DateTime.UtcNow.AddHours(2);
        var endDate = startDate.AddHours(-1);

        var invalidDto = new UpdateBookingDto(null, null, startDate, endDate);

        var result = validator.Validate(invalidDto);

        Assert.False(result.IsValid);
    }
}