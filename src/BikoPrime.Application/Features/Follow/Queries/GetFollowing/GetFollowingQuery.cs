namespace BikoPrime.Application.Features.Follow.Queries.GetFollowing;

using MediatR;
using BikoPrime.Application.DTOs.User;

public class GetFollowingQuery : IRequest<List<UserProfileDto>>
{
    public Guid UserId { get; set; }

    public Guid? CurrentUserId { get; set; }
}
