namespace BikoPrime.Application.Features.Users.Queries.GetRecommendedUsers;

using MediatR;
using BikoPrime.Application.DTOs.User;

public class GetRecommendedUsersQuery : IRequest<List<UserProfileDto>>
{
    public string? CurrentUserId { get; set; }

    public int PageNumber { get; set; } = 1;

    public int PageSize { get; set; } = 10;
}
