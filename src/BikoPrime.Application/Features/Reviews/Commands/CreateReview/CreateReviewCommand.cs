namespace BikoPrime.Application.Features.Reviews.Commands.CreateReview;

using MediatR;
using BikoPrime.Application.DTOs.Review;

public class CreateReviewCommand : IRequest<ReviewDto>
{
    public Guid ContractId { get; set; }

    public Guid TargetUserId { get; set; }

    public int Rating { get; set; }

    public string Comment { get; set; } = string.Empty;

    public string ReviewerId { get; set; } = string.Empty;

    public string RevieweeId { get; set; } = string.Empty;
}
