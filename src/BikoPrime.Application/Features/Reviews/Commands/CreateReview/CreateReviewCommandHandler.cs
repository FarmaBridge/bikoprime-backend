using MediatR;
using BikoPrime.Application.DTOs.Review;
using BikoPrime.Application.Interfaces;
using BikoPrime.Domain.Entities;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace BikoPrime.Application.Features.Reviews.Commands.CreateReview;

public class CreateReviewCommandHandler : IRequestHandler<CreateReviewCommand, ReviewDto>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IContractRepository _contractRepository;
    private readonly IUserRepository _userRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CreateReviewCommandHandler(IReviewRepository reviewRepository, IContractRepository contractRepository, IUserRepository userRepository, IHttpContextAccessor httpContextAccessor)
    {
        _reviewRepository = reviewRepository;
        _contractRepository = contractRepository;
        _userRepository = userRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<ReviewDto> Handle(CreateReviewCommand request, CancellationToken cancellationToken)
    {
        var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var parsedUserId))
        {
            throw new UnauthorizedAccessException("User not authenticated");
        }

        var contract = await _contractRepository.GetByIdAsync(request.ContractId);
        if (contract == null)
            throw new KeyNotFoundException($"Contract with id {request.ContractId} not found");

        if (contract.Status != "finished")
            throw new InvalidOperationException("Can only review finished contracts");

        // Only provider or client can create review
        if (contract.ProviderId != parsedUserId && contract.ClientId != parsedUserId)
            throw new UnauthorizedAccessException("You are not authorized to create a review for this contract");

        // Cannot review yourself
        if (parsedUserId == request.TargetUserId)
            throw new InvalidOperationException("You cannot review yourself");

        // Check if review already exists for this contract and user combination
        var existingReview = await _reviewRepository.GetByContractAndTargetUserAsync(request.ContractId, request.TargetUserId);
        if (existingReview != null)
            throw new InvalidOperationException("A review for this contract and user already exists");

        var review = new Review
        {
            Id = Guid.NewGuid(),
            ContractId = request.ContractId,
            AuthorId = parsedUserId,
            TargetUserId = request.TargetUserId,
            Rating = request.Rating,
            Comment = request.Comment,
            CreatedAt = DateTime.UtcNow
        };

        await _reviewRepository.CreateAsync(review);

        // Update user's rating (average of all reviews)
        var targetUser = await _userRepository.GetByIdAsync(request.TargetUserId);
        if (targetUser != null)
        {
            var allReviews = await _reviewRepository.GetByTargetUserIdAsync(request.TargetUserId);
            var averageRating = allReviews.Any() ? (decimal)allReviews.Average(r => r.Rating) : 0;
            targetUser.Rating = averageRating;
            await _userRepository.UpdateAsync(targetUser);
        }

        return MapToDto(review);
    }

    private ReviewDto MapToDto(Review review)
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
