namespace BikoPrime.Application.Features.Demands.Commands.UpdateDemand;

using FluentValidation;

public class UpdateDemandCommandValidator : AbstractValidator<UpdateDemandCommand>
{
    public UpdateDemandCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Demand ID is required");

        RuleFor(x => x.Title)
            .MaximumLength(500).WithMessage("Title must not exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.Title));

        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.Budget)
            .GreaterThan(0).WithMessage("Budget must be greater than 0")
            .When(x => x.Budget.HasValue);

        RuleFor(x => x.ServiceRadiusKm)
            .GreaterThan(0).WithMessage("Service radius must be greater than 0")
            .When(x => x.ServiceRadiusKm.HasValue);

        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90, 90).WithMessage("Latitude must be between -90 and 90")
            .When(x => x.Latitude.HasValue);

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180, 180).WithMessage("Longitude must be between -180 and 180")
            .When(x => x.Longitude.HasValue);
    }
}
