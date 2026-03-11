namespace BikoPrime.Application.Features.Users.Queries.GetUserReviews;

using MediatR;
using BikoPrime.Application.DTOs.Review;
using BikoPrime.Application.Interfaces;

public class GetUserReviewsQueryHandler : IRequestHandler<GetUserReviewsQuery, List<ReviewDto>>
{
    private readonly IReviewRepository _reviewRepository;

    public GetUserReviewsQueryHandler(IReviewRepository reviewRepository)
    {
        _reviewRepository = reviewRepository;
    }

    public async Task<List<ReviewDto>> Handle(GetUserReviewsQuery request, CancellationToken cancellationToken)
    {
        var reviews = await _reviewRepository.GetByTargetUserIdAsync(Guid.Parse(request.UserId));
        return reviews.Select(MapToDto).ToList();
    }

    private ReviewDto MapToDto(BikoPrime.Domain.Entities.Review review)
    {
        return new ReviewDto
        {
            Id = review.Id,
            ContractId = review.ContractId,
            AuthorId = review.AuthorId,
            TargetUserId = review.TargetUserId,
            Rating = review.Rating,
            Comment = review.Comment,
            CreatedAt = review.CreatedAt
        };
    }
}
