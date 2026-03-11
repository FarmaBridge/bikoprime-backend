namespace BikoPrime.Domain.Entities;

public class Service
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public string? Subcategory { get; set; }

    public string PriceType { get; set; } = string.Empty; // fixed, hourly, negotiable

    public decimal Price { get; set; }

    public List<string> Photos { get; set; } = new List<string>();

    public Guid ProviderId { get; set; }

    public double Latitude { get; set; }

    public double Longitude { get; set; }

    public string? Address { get; set; }

    public decimal ServiceRadiusKm { get; set; }

    public decimal Rating { get; set; } = 0;

    public int ReviewsCount { get; set; } = 0;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    // Navigation
    public User? Provider { get; set; }

    public List<Contract> Contracts { get; set; } = new List<Contract>();
}
