using BadmintonClubAPI.Models.DTOs.Matches;
using BadmintonClubAPI.Validators;

namespace BadmintonClubAPI.Tests.Validators;

public class MatchValidatorTests
{
    [Fact]
    public void CreateMatchDtoValidator_WhenNoParticipants_ShouldHaveValidationError()
    {
        var validator = new CreateMatchDtoValidator();
        var invalidDto = new CreateMatchDto(DateTime.UtcNow, DateTime.UtcNow.AddHours(1), null, new List<MatchParticipantDto>());

        var result = validator.Validate(invalidDto);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateMatchDto.Participants));
    }
}