using BadmintonClubAPI.Models.DTOs.Matches;
using FluentValidation;

namespace BadmintonClubAPI.Validators;

public class CreateMatchDtoValidator : AbstractValidator<CreateMatchDto>
{
    public CreateMatchDtoValidator()
    {
        RuleFor(x => x.StartDate).NotEmpty();
        RuleFor(x => x.EndDate).GreaterThan(x => x.StartDate);
        RuleFor(x => x.Participants).NotEmpty().WithMessage("A match must have at least one participant.");
    }
}