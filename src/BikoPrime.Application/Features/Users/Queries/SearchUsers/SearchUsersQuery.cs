namespace BikoPrime.Application.Features.Users.Queries.SearchUsers;

using MediatR;
using BikoPrime.Application.DTOs.User;

public class SearchUsersQuery : IRequest<List<UserProfileDto>>
{
    public string Query { get; set; } = string.Empty;

    public string? Keyword { get; set; }

    public string? Location { get; set; }

    public string? CurrentUserId { get; set; }
}
