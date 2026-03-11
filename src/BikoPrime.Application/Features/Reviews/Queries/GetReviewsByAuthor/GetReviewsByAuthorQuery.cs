namespace BikoPrime.Application.Features.Reviews.Queries.GetReviewsByAuthor;

using MediatR;
using BikoPrime.Application.DTOs.Review;

public class GetReviewsByAuthorQuery : IRequest<List<ReviewDto>>
{
    public string? AuthorId { get; set; }

    public int PageNumber { get; set; }

    public int PageSize { get; set; }
}
