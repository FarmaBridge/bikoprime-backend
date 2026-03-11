namespace BikoPrime.Application.DTOs.Service;

using BikoPrime.Application.DTOs.Auth;

public class ServiceDto
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public string? Subcategory { get; set; }

    public string PriceType { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public List<string> Photos { get; set; } = new List<string>();

    public Guid ProviderId { get; set; }

    public LocationDto Location { get; set; } = new();

    public decimal ServiceRadiusKm { get; set; }

    public decimal Rating { get; set; }

    public int ReviewsCount { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
