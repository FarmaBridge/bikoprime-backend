namespace BikoPrime.Application.Features.Services.Queries.GetServicesByProvider;

using MediatR;
using BikoPrime.Application.DTOs.Service;

public class GetServicesByProviderQuery : IRequest<List<ServiceDto>>
{
    public Guid ProviderId { get; set; }
}
