namespace BikoPrime.Application.Features.Demands.Queries.GetNearbyDemands;

using MediatR;
using BikoPrime.Application.DTOs.Demand;

public class GetNearbyDemandsQuery : IRequest<List<DemandDto>>
{
    public double Latitude { get; set; }

    public double Longitude { get; set; }

    public decimal RadiusKm { get; set; }
}
