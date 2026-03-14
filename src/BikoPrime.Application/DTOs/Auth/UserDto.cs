namespace BikoPrime.Application.DTOs.Auth;

public class UserDto
{
    public Guid Id { get; set; }
    
    public string FirstName { get; set; } = string.Empty;
    
    public string LastName { get; set; } = string.Empty;
    
    public string DisplayName { get; set; } = string.Empty;
    
    public string UserName { get; set; } = string.Empty;
    
    public string Email { get; set; } = string.Empty;
    
    public string PhoneNumber { get; set; } = string.Empty;

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
    
    public LocationDto Location { get; set; } = new();
    
    public decimal Rating { get; set; }
    
    public int FollowersCount { get; set; }
    
    public int FollowingCount { get; set; }
    
    public string? Bio { get; set; }
    
    public ICollection<string> Services { get; set; } = new List<string>();
}
