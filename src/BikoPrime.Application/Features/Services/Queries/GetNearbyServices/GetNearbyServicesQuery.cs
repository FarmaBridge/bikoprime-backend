namespace BikoPrime.Application.Features.Services.Queries.GetNearbyServices;

using MediatR;
using BikoPrime.Application.DTOs.Service;

public class GetNearbyServicesQuery : IRequest<List<ServiceDto>>
{
    public double Latitude { get; set; }

    public double Longitude { get; set; }

    public decimal RadiusKm { get; set; }
}
