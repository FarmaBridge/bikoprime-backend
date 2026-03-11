namespace BikoPrime.Application.Features.Reviews.Queries.GetReviewsByAuthor;

using MediatR;
using BikoPrime.Application.DTOs.Review;
using BikoPrime.Application.Interfaces;

public class GetReviewsByAuthorQueryHandler : IRequestHandler<GetReviewsByAuthorQuery, List<ReviewDto>>
{
    private readonly IReviewRepository _reviewRepository;

    public GetReviewsByAuthorQueryHandler(IReviewRepository reviewRepository)
    {
        _reviewRepository = reviewRepository;
    }

    public async Task<List<ReviewDto>> Handle(GetReviewsByAuthorQuery request, CancellationToken cancellationToken)
    {
        var reviews = await _reviewRepository.GetByAuthorIdAsync(Guid.Parse(request.AuthorId!));
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
