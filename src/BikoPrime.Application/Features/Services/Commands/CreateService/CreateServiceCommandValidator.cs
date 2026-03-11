namespace BikoPrime.Application.Features.Services.Commands.CreateService;

using FluentValidation;

public class CreateServiceCommandValidator : AbstractValidator<CreateServiceCommand>
{
    public CreateServiceCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(500).WithMessage("Title must not exceed 500 characters");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required")
            .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters");

        RuleFor(x => x.Category)
            .NotEmpty().WithMessage("Category is required");

        RuleFor(x => x.PriceType)
            .NotEmpty().WithMessage("Price type is required")
            .Must(x => x == "fixed" || x == "hourly" || x == "negotiable")
            .WithMessage("Price type must be 'fixed', 'hourly', or 'negotiable'");

        RuleFor(x => x.Price)
            .GreaterThan(0).When(x => x.PriceType == "fixed" || x.PriceType == "hourly")
            .WithMessage("Price must be greater than 0");

        RuleFor(x => x.ServiceRadiusKm)
            .GreaterThan(0).WithMessage("Service radius must be greater than 0");
    }
}
