namespace BikoPrime.Application.DTOs.Demand;

using BikoPrime.Application.DTOs.Auth;

public class DemandDto
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public string? Subcategory { get; set; }

    public decimal Budget { get; set; }

    public List<string> Photos { get; set; } = new List<string>();

    public LocationDto Location { get; set; } = new();

    public decimal ServiceRadiusKm { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
