namespace BikoPrime.Domain.Entities;

using Microsoft.AspNetCore.Identity;

public class User : IdentityUser<Guid>
{
    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? DisplayName { get; set; }

    public string? Gender { get; set; }

    public string? Pronoun { get; set; }

    public DateTime? DateOfBirth { get; set; }

    public string? CEP { get; set; }

    public string? Street { get; set; }

    public string? StreetNumber { get; set; }

    public string? Complement { get; set; }

    public string? Neighborhood { get; set; }

    public string? City { get; set; }

    public string? State { get; set; }

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
