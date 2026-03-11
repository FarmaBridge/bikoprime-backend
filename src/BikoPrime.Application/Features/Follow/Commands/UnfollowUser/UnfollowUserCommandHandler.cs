using MediatR;
using BikoPrime.Application.Interfaces;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace BikoPrime.Application.Features.Follow.Commands.UnfollowUser;

public class UnfollowUserCommandHandler : IRequestHandler<UnfollowUserCommand, Unit>
{
    private readonly IUserFollowRepository _userFollowRepository;
    private readonly IUserRepository _userRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UnfollowUserCommandHandler(IUserFollowRepository userFollowRepository, IUserRepository userRepository, IHttpContextAccessor httpContextAccessor)
    {
        _userFollowRepository = userFollowRepository;
        _userRepository = userRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Unit> Handle(UnfollowUserCommand request, CancellationToken cancellationToken)
    {
        var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var parsedUserId))
        {
            throw new UnauthorizedAccessException("User not authenticated");
        }

        if (string.IsNullOrEmpty(request.FollowingId) || !Guid.TryParse(request.FollowingId, out var targetUserId))
        {
            throw new InvalidOperationException("Invalid target user ID");
        }

        var userFollow = await _userFollowRepository.GetFollowAsync(parsedUserId, targetUserId);
        if (userFollow == null)
            throw new KeyNotFoundException("You are not following this user");

        await _userFollowRepository.DeleteAsync(userFollow.Id);

        // Update follower/following counts
        var targetUser = await _userRepository.GetByIdAsync(targetUserId);
        if (targetUser != null)
        {
            targetUser.FollowersCount = Math.Max(0, targetUser.FollowersCount - 1);
            await _userRepository.UpdateAsync(targetUser);
        }

        var follower = await _userRepository.GetByIdAsync(parsedUserId);
        if (follower != null)
        {
            follower.FollowingCount = Math.Max(0, follower.FollowingCount - 1);
            await _userRepository.UpdateAsync(follower);
        }

        return Unit.Value;
    }
}
