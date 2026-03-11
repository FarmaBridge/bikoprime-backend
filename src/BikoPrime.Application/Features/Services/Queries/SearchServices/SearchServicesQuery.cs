namespace BikoPrime.Application.Features.Services.Queries.SearchServices;

using MediatR;
using BikoPrime.Application.DTOs.Service;

public class SearchServicesQuery : IRequest<List<ServiceDto>>
{
    public string Category { get; set; } = string.Empty;

    public string? Subcategory { get; set; }

    public string? Keyword { get; set; }

    public decimal? MinPrice { get; set; }

    public decimal? MaxPrice { get; set; }
}
