namespace BikoPrime.Application.Features.Users.Queries.GetUserReviews;

using MediatR;
using BikoPrime.Application.DTOs.Review;

public class GetUserReviewsQuery : IRequest<List<ReviewDto>>
{
    public string UserId { get; set; } = string.Empty;
}
