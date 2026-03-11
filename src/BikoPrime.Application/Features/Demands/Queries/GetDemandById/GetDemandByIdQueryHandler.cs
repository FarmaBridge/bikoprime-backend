namespace BikoPrime.Application.Features.Demands.Queries.GetDemandById;

using MediatR;
using BikoPrime.Application.DTOs.Demand;
using BikoPrime.Application.Interfaces;
using BikoPrime.Application.DTOs.Auth;

public class GetDemandByIdQueryHandler : IRequestHandler<GetDemandByIdQuery, DemandDto>
{
    private readonly IDemandRepository _demandRepository;

    public GetDemandByIdQueryHandler(IDemandRepository demandRepository)
    {
        _demandRepository = demandRepository;
    }

    public async Task<DemandDto> Handle(GetDemandByIdQuery request, CancellationToken cancellationToken)
    {
        var demand = await _demandRepository.GetByIdAsync(request.Id);
        if (demand == null)
            throw new KeyNotFoundException($"Demand with id {request.Id} not found");

        return MapToDto(demand);
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
