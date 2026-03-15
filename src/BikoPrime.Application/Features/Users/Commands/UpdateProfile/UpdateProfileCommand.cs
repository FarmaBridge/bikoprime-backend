namespace BikoPrime.Application.Features.Users.Commands.UpdateProfile;

using MediatR;
using BikoPrime.Application.DTOs.User;

public class UpdateProfileCommand : IRequest<UserProfileDto>
{
    public string? UserId { get; set; }

    public string? Name { get; set; }

    public string? UserName { get; set; }

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public double? Latitude { get; set; }

    public double? Longitude { get; set; }

    public string? Address { get; set; }

    public string? Bio { get; set; }
}
