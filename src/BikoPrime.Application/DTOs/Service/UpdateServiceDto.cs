namespace BikoPrime.Application.DTOs.Service;

using BikoPrime.Application.DTOs.Auth;

public class UpdateServiceDto
{
    public string? Title { get; set; }

    public string? Description { get; set; }

    public string? Category { get; set; }

    public string? Subcategory { get; set; }

    public string? PriceType { get; set; }

    public decimal? Price { get; set; }

    public List<string>? Photos { get; set; }

    public LocationDto? Location { get; set; }

    public decimal? ServiceRadiusKm { get; set; }
}
