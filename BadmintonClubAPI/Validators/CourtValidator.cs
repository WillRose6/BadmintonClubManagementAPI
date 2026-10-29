using BadmintonClubAPI.Models.DTOs.Courts;
using FluentValidation;

namespace BadmintonClubAPI.Validators;

public class CreateCourtDtoValidator : AbstractValidator<CreateCourtDto>
{
    public CreateCourtDtoValidator()
    {
        RuleFor(x => x.CourtIndex).GreaterThan(0);
    }
}

public class UpdateCourtDtoValidator : AbstractValidator<UpdateCourtDto>
{
    public UpdateCourtDtoValidator()
    {
        RuleFor(x => x.Index)
            .GreaterThan(0)
            .When(x => x.Index.HasValue);
    }
}