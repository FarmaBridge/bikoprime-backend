namespace BikoPrime.Application.Features.Reviews.Commands.CreateReview;

using FluentValidation;

public class CreateReviewCommandValidator : AbstractValidator<CreateReviewCommand>
{
    public CreateReviewCommandValidator()
    {
        RuleFor(x => x.ContractId)
            .NotEmpty().WithMessage("Contract ID is required");

        RuleFor(x => x.TargetUserId)
            .NotEmpty().WithMessage("Target user ID is required");

        RuleFor(x => x.Rating)
            .InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5");

        RuleFor(x => x.Comment)
            .MaximumLength(1000).WithMessage("Comment must not exceed 1000 characters");
    }
}
