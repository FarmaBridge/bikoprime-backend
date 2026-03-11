namespace BikoPrime.Application.Features.Services.Commands.CreateService;

using MediatR;
using BikoPrime.Application.DTOs.Service;

public class CreateServiceCommand : IRequest<ServiceDto>
{
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public string? Subcategory { get; set; }

    public string PriceType { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public List<string> Photos { get; set; } = new List<string>();

    public double Latitude { get; set; }

    public double Longitude { get; set; }

    public string? Address { get; set; }

    public decimal ServiceRadiusKm { get; set; }

    public string ProviderId { get; set; } = string.Empty;
}
