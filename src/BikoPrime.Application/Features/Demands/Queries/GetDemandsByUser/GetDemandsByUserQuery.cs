namespace BikoPrime.Application.Features.Demands.Queries.GetDemandsByUser;

using MediatR;
using BikoPrime.Application.DTOs.Demand;

public class GetDemandsByUserQuery : IRequest<List<DemandDto>>
{
    public Guid UserId { get; set; }
}
