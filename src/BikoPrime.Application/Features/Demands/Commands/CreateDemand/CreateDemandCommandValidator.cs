namespace BikoPrime.Application.Features.Demands.Commands.CreateDemand;

using FluentValidation;

public class CreateDemandCommandValidator : AbstractValidator<CreateDemandCommand>
{
    public CreateDemandCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(500).WithMessage("Title must not exceed 500 characters");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required")
            .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters");

        RuleFor(x => x.Category)
            .NotEmpty().WithMessage("Category is required");

        RuleFor(x => x.Budget)
            .GreaterThan(0).WithMessage("Budget must be greater than 0");

        RuleFor(x => x.ServiceRadiusKm)
            .GreaterThan(0).WithMessage("Service radius must be greater than 0");

        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90, 90).WithMessage("Latitude must be between -90 and 90");

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180, 180).WithMessage("Longitude must be between -180 and 180");
    }
}
