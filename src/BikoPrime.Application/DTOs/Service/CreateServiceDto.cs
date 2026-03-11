namespace BikoPrime.Application.DTOs.Service;

using BikoPrime.Application.DTOs.Auth;

public class CreateServiceDto
{
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public string? Subcategory { get; set; }

    public string PriceType { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public List<string> Photos { get; set; } = new List<string>();

    public LocationDto Location { get; set; } = new();

    public decimal ServiceRadiusKm { get; set; }
}
