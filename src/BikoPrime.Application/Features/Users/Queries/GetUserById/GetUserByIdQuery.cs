namespace BikoPrime.Application.Features.Users.Queries.GetUserById;

using MediatR;
using BikoPrime.Application.DTOs.User;

public class GetUserByIdQuery : IRequest<UserProfileDto>
{
    public string UserId { get; set; } = string.Empty;

    public string? CurrentUserId { get; set; }
}
