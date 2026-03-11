namespace BikoPrime.Application.DTOs.User;

using BikoPrime.Application.DTOs.Auth;

public class UpdateProfileDto
{
    public string? Name { get; set; }

    public string? UserName { get; set; }

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public string? AvatarUrl { get; set; }

    public LocationDto? Location { get; set; }

    public string? Bio { get; set; }
}
