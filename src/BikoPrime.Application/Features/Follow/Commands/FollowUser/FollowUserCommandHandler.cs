using MediatR;
using BikoPrime.Application.Interfaces;
using BikoPrime.Domain.Entities;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace BikoPrime.Application.Features.Follow.Commands.FollowUser;

public class FollowUserCommandHandler : IRequestHandler<FollowUserCommand, Unit>
{
    private readonly IUserFollowRepository _userFollowRepository;
    private readonly IUserRepository _userRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public FollowUserCommandHandler(IUserFollowRepository userFollowRepository, IUserRepository userRepository, IHttpContextAccessor httpContextAccessor)
    {
        _userFollowRepository = userFollowRepository;
        _userRepository = userRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Unit> Handle(FollowUserCommand request, CancellationToken cancellationToken)
    {
        var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var parsedUserId))
        {
            throw new UnauthorizedAccessException("User not authenticated");
        }

        if (parsedUserId == request.TargetUserId)
            throw new InvalidOperationException("You cannot follow yourself");

        var targetUser = await _userRepository.GetByIdAsync(request.TargetUserId);
        if (targetUser == null)
            throw new KeyNotFoundException($"User with id {request.TargetUserId} not found");

        var isAlreadyFollowing = await _userFollowRepository.IsFollowingAsync(parsedUserId, request.TargetUserId);
        if (isAlreadyFollowing)
            throw new InvalidOperationException("You are already following this user");

        var userFollow = new UserFollow
        {
            Id = Guid.NewGuid(),
            FollowerId = parsedUserId,
            FollowingId = request.TargetUserId,
            CreatedAt = DateTime.UtcNow
        };

        await _userFollowRepository.CreateAsync(userFollow);

        // Update follower/following counts
        targetUser.FollowersCount++;
        var follower = await _userRepository.GetByIdAsync(parsedUserId);
        if (follower != null)
        {
            follower.FollowingCount++;
            await _userRepository.UpdateAsync(follower);
        }
        await _userRepository.UpdateAsync(targetUser);

        return Unit.Value;
    }
}
