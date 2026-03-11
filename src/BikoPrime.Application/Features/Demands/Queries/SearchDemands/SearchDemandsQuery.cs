namespace BikoPrime.Application.Features.Demands.Queries.SearchDemands;

using MediatR;
using BikoPrime.Application.DTOs.Demand;

public class SearchDemandsQuery : IRequest<List<DemandDto>>
{
    public string? Keyword { get; set; }

    public string? Category { get; set; }

    public string? Subcategory { get; set; }

    public decimal? MinBudget { get; set; }

    public decimal? MaxBudget { get; set; }
}
