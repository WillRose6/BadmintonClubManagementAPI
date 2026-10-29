using BadmintonClubAPI.Models.DTOs.Players;
using BadmintonClubAPI.Validators;

namespace BadmintonClubAPI.Tests.Validators;

public class PlayerValidatorTests
{
    [Fact]
    public void CreatePlayerDtoValidator_WhenValid_ShouldPass()
    {
        var validator = new CreatePlayerDtoValidator();
        var validDto = new CreatePlayerDto("Daniella", "Smith", new DateTime(2000, 5, 20), "daniella@gmail.com", "Password123!");

        var result = validator.Validate(validDto);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void CreatePlayerDtoValidator_WhenEmailIsInvalid_ShouldHaveValidationError()
    {
        var validator = new CreatePlayerDtoValidator();
        var invalidDto = new CreatePlayerDto("Daniella", "Smith", new DateTime(2000, 5, 20), "not-an-email", "Password123!");

        var result = validator.Validate(invalidDto);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreatePlayerDto.EmailAddress));
    }

    [Fact]
    public void CreatePlayerDtoValidator_WhenPasswordTooShort_ShouldHaveValidationError()
    {
        var validator = new CreatePlayerDtoValidator();
        var invalidDto = new CreatePlayerDto("Daniella", "Smith", new DateTime(2000, 5, 20), "daniella@gmail.com", "123");

        var result = validator.Validate(invalidDto);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreatePlayerDto.Password));
    }

    [Fact]
    public void CreatePlayerDtoValidator_WhenDateOfBirthInFuture_ShouldHaveValidationError()
    {
        var validator = new CreatePlayerDtoValidator();
        var invalidDto = new CreatePlayerDto("Daniella", "Smith", DateTime.UtcNow.AddYears(1), "daniella@gmail.com", "Password123!");

        var result = validator.Validate(invalidDto);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreatePlayerDto.DateOfBirth));
    }

    [Fact]
    public void UpdatePlayerDtoValidator_WhenFieldsSuppliedAndValid_ShouldPass()
    {
        var validator = new UpdatePlayerDtoValidator();
        var validDto = new UpdatePlayerDto("Daniella", "Smith", "daniella@gmail.com", new DateTime(2000, 5, 20));

        var result = validator.Validate(validDto);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void UpdatePlayerDtoValidator_WhenAllFieldsNull_ShouldPass()
    {
        var validator = new UpdatePlayerDtoValidator();
        var validDto = new UpdatePlayerDto(null, null, null, null);

        var result = validator.Validate(validDto);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void UpdatePlayerDtoValidator_WhenInvalidEmail_ShouldHaveValidationError()
    {
        var validator = new UpdatePlayerDtoValidator();
        var invalidDto = new UpdatePlayerDto(null, null, "bad-email", null);

        var result = validator.Validate(invalidDto);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdatePlayerDto.EmailAddress));
    }
}