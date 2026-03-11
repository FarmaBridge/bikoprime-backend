using MediatR;
using BikoPrime.Application.DTOs.Demand;
using BikoPrime.Application.Interfaces;
using BikoPrime.Domain.Entities;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using BikoPrime.Application.DTOs.Auth;

namespace BikoPrime.Application.Features.Demands.Commands.CreateDemand;

public class CreateDemandCommandHandler : IRequestHandler<CreateDemandCommand, DemandDto>
{
    private readonly IDemandRepository _demandRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CreateDemandCommandHandler(IDemandRepository demandRepository, IHttpContextAccessor httpContextAccessor)
    {
        _demandRepository = demandRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<DemandDto> Handle(CreateDemandCommand request, CancellationToken cancellationToken)
    {
        var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var parsedUserId))
        {
            throw new UnauthorizedAccessException("User not authenticated");
        }

        var demand = new Demand
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            Category = request.Category,
            Subcategory = request.Subcategory,
            Budget = request.Budget,
            Photos = request.Photos,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            Address = request.Address,
            ServiceRadiusKm = request.ServiceRadiusKm,
            CreatedBy = parsedUserId,
            CreatedAt = DateTime.UtcNow
        };

        await _demandRepository.CreateAsync(demand);

        return MapToDto(demand);
    }

    private DemandDto MapToDto(Demand demand)
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
