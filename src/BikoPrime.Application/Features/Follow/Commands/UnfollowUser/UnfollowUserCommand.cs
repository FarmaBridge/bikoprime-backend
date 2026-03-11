namespace BikoPrime.Application.Features.Follow.Commands.UnfollowUser;

using MediatR;

public class UnfollowUserCommand : IRequest<Unit>
{
    public Guid TargetUserId { get; set; }

    public string? FollowerId { get; set; }

    public string? FollowingId { get; set; }
}
