namespace BikoPrime.Domain.Entities;

public class Demand
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public string? Subcategory { get; set; }

    public decimal Budget { get; set; }

    public List<string> Photos { get; set; } = new List<string>();

    public double Latitude { get; set; }

    public double Longitude { get; set; }

    public string? Address { get; set; }

    public decimal ServiceRadiusKm { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    // Navigation
    public User? Creator { get; set; }

    public List<Contract> Contracts { get; set; } = new List<Contract>();
}
