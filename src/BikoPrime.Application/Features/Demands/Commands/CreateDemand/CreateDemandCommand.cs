namespace BikoPrime.Application.Features.Demands.Commands.CreateDemand;

using MediatR;
using BikoPrime.Application.DTOs.Demand;

public class CreateDemandCommand : IRequest<DemandDto>
{
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

    public string UserId { get; set; } = string.Empty;
}
