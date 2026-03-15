namespace BikoPrime.Application.DTOs.User;

using BikoPrime.Application.DTOs.Auth;

public class UserProfileDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;

    public string UserName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public Guid? PhotoId { get; set; }

    public LocationDto Location { get; set; } = new();

    public decimal Rating { get; set; }

    public int FollowersCount { get; set; }

    public int FollowingCount { get; set; }

    public string? Bio { get; set; }

    public ICollection<string> Services { get; set; } = new List<string>();

    public bool IsFollowing { get; set; }
}
