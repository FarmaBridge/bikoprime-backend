namespace BikoPrime.Application.Features.Follow.Commands.FollowUser;

using MediatR;

public class FollowUserCommand : IRequest<Unit>
{
    public Guid TargetUserId { get; set; }

    public string? FollowerId { get; set; }

    public string? FollowingId { get; set; }
}
