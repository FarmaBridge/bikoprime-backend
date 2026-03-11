namespace BikoPrime.Domain.Entities;

using Microsoft.AspNetCore.Identity;

public class User : IdentityUser<Guid>
{
    public string? AvatarUrl { get; set; }

    public double Latitude { get; set; }

    public double Longitude { get; set; }

    public string? Address { get; set; }

    public decimal Rating { get; set; } = 0;

    public int FollowersCount { get; set; } = 0;

    public int FollowingCount { get; set; } = 0;

    public string? Bio { get; set; }

    public List<string>? Services { get; set; } = new List<string>();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }
}
