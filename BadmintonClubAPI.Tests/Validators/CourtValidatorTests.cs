using BadmintonClubAPI.Models.DTOs.Courts;
using BadmintonClubAPI.Validators;

namespace BadmintonClubAPI.Tests.Validators;

public class CourtValidatorTests
{
    [Fact]
    public void CreateCourtDtoValidator_WhenCourtIndexIsZeroOrNegative_ShouldHaveValidationError()
    {
        var validator = new CreateCourtDtoValidator();
        var invalidDto = new CreateCourtDto(0);

        var result = validator.Validate(invalidDto);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateCourtDto.CourtIndex));
    }

    [Fact]
    public void UpdateCourtDtoValidator_WhenIndexInvalid_ShouldHaveValidationError()
    {
        var validator = new UpdateCourtDtoValidator();
        var invalidDto = new UpdateCourtDto(-1, true);

        var result = validator.Validate(invalidDto);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateCourtDto.Index));
    }
}