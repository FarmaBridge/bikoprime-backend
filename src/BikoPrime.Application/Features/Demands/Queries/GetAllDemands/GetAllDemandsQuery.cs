namespace BikoPrime.Application.Features.Demands.Queries.GetAllDemands;

using MediatR;
using BikoPrime.Application.DTOs.Demand;

public class GetAllDemandsQuery : IRequest<List<DemandDto>>
{
}
