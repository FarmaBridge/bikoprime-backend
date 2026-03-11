namespace BikoPrime.Application.Features.Demands.Queries.GetDemandById;

using MediatR;
using BikoPrime.Application.DTOs.Demand;

public class GetDemandByIdQuery : IRequest<DemandDto>
{
    public Guid Id { get; set; }
}
