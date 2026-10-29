using BadmintonClubAPI.Models.DTOs.Leagues;
using FluentValidation;

namespace BadmintonClubAPI.Validators;

public class CreateLeagueDtoValidator : AbstractValidator<CreateLeagueDto>
{
    public CreateLeagueDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.StartDate).NotEmpty();
        RuleFor(x => x.EndDate).GreaterThan(x => x.StartDate).WithMessage("League end date must be after start date.");
    }
}