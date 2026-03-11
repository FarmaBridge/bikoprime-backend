namespace BikoPrime.Application.Features.Demands.Queries.GetDemandsByCategory;

using MediatR;
using BikoPrime.Application.DTOs.Demand;
using BikoPrime.Application.Interfaces;
using BikoPrime.Application.DTOs.Auth;

public class GetDemandsByCategoryQueryHandler : IRequestHandler<GetDemandsByCategoryQuery, List<DemandDto>>
{
    private readonly IDemandRepository _demandRepository;

    public GetDemandsByCategoryQueryHandler(IDemandRepository demandRepository)
    {
        _demandRepository = demandRepository;
    }

    public async Task<List<DemandDto>> Handle(GetDemandsByCategoryQuery request, CancellationToken cancellationToken)
    {
        var demands = await _demandRepository.GetByCategoryAsync(request.Category);
        return demands.Select(MapToDto).ToList();
    }

    private DemandDto MapToDto(BikoPrime.Domain.Entities.Demand demand)
    {
        return new DemandDto
        {
            Id = demand.Id,
            Title = demand.Title,
            Description = demand.Description,
            Category = demand.Category,
            Subcategory = demand.Subcategory,
            Budget = demand.Budget,
            Photos = demand.Photos,
            Location = new LocationDto
            {
                Latitude = demand.Latitude,
                Longitude = demand.Longitude,
                Address = demand.Address
            },
            ServiceRadiusKm = demand.ServiceRadiusKm,
            CreatedBy = demand.CreatedBy,
            CreatedAt = demand.CreatedAt,
            UpdatedAt = demand.UpdatedAt
        };
    }
}
