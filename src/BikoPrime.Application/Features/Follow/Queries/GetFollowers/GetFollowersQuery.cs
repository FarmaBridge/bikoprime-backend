namespace BikoPrime.Application.Features.Follow.Queries.GetFollowers;

using MediatR;
using BikoPrime.Application.DTOs.User;

public class GetFollowersQuery : IRequest<List<UserProfileDto>>
{
    public Guid UserId { get; set; }

    public Guid? CurrentUserId { get; set; }
}
