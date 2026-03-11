namespace BikoPrime.Application.Features.Demands.Queries.GetDemandsByCategory;

using MediatR;
using BikoPrime.Application.DTOs.Demand;

public class GetDemandsByCategoryQuery : IRequest<List<DemandDto>>
{
    public string Category { get; set; } = string.Empty;
}
