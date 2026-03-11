namespace BikoPrime.Application.Features.Demands.Commands.UpdateDemand;

using MediatR;
using BikoPrime.Application.DTOs.Demand;

public class UpdateDemandCommand : IRequest<DemandDto>
{
    public Guid Id { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public string? Category { get; set; }

    public string? Subcategory { get; set; }

    public decimal? Budget { get; set; }

    public List<string>? Photos { get; set; }

    public double? Latitude { get; set; }

    public double? Longitude { get; set; }

    public string? Address { get; set; }

    public decimal? ServiceRadiusKm { get; set; }

    public string? UserId { get; set; }
}
