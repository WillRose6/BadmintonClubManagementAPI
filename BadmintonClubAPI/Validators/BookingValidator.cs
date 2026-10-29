using BadmintonClubAPI.Models.DTOs.Bookings;
using FluentValidation;

namespace BadmintonClubAPI.Validators;

public class CreateBookingDtoValidator : AbstractValidator<CreateBookingDto>
{
    public CreateBookingDtoValidator()
    {
        RuleFor(x => x.PlayerId).GreaterThan(0);
        RuleFor(x => x.CourtId).GreaterThan(0);
        RuleFor(x => x.StartDate).NotEmpty();
        RuleFor(x => x.EndDate).GreaterThan(x => x.StartDate).WithMessage("End date must be after start date.");
    }
}

public class UpdateBookingDtoValidator : AbstractValidator<UpdateBookingDto>
{
    public UpdateBookingDtoValidator()
    {
        RuleFor(x => x.PlayerId)
            .GreaterThan(0)
            .When(x => x.PlayerId.HasValue);

        RuleFor(x => x.CourtId)
            .GreaterThan(0)
            .When(x => x.CourtId.HasValue);

        RuleFor(x => x)
            .Must(x => x.EndDate > x.StartDate)
            .WithMessage("End date must be after start date when updating times.")
            .When(x => x.StartDate.HasValue && x.EndDate.HasValue);
    }
}