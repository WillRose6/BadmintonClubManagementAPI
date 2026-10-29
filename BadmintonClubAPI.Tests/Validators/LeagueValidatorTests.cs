using BadmintonClubAPI.Models.DTOs.Leagues;
using BadmintonClubAPI.Validators;

namespace BadmintonClubAPI.Tests.Validators;

public class LeagueValidatorTests
{
    [Fact]
    public void CreateLeagueDtoValidator_WhenValid_ShouldPass()
    {
        var validator = new CreateLeagueDtoValidator();
        var validDto = new CreateLeagueDto("Premier Division", DateTime.UtcNow, DateTime.UtcNow.AddMonths(3));

        var result = validator.Validate(validDto);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void CreateLeagueDtoValidator_WhenNameEmpty_ShouldHaveValidationError()
    {
        var validator = new CreateLeagueDtoValidator();
        var invalidDto = new CreateLeagueDto("", DateTime.UtcNow, DateTime.UtcNow.AddMonths(3));

        var result = validator.Validate(invalidDto);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateLeagueDto.Name));
    }

    [Fact]
    public void CreateLeagueDtoValidator_WhenEndDateBeforeStartDate_ShouldHaveValidationError()
    {
        var validator = new CreateLeagueDtoValidator();
        var invalidDto = new CreateLeagueDto("Division 1", DateTime.UtcNow.AddDays(10), DateTime.UtcNow);

        var result = validator.Validate(invalidDto);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateLeagueDto.EndDate));
    }
}