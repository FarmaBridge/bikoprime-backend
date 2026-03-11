using MediatR;
using BikoPrime.Application.DTOs.Demand;
using BikoPrime.Application.Interfaces;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using BikoPrime.Application.DTOs.Auth;

namespace BikoPrime.Application.Features.Demands.Commands.UpdateDemand;

public class UpdateDemandCommandHandler : IRequestHandler<UpdateDemandCommand, DemandDto>
{
    private readonly IDemandRepository _demandRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UpdateDemandCommandHandler(IDemandRepository demandRepository, IHttpContextAccessor httpContextAccessor)
    {
        _demandRepository = demandRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<DemandDto> Handle(UpdateDemandCommand request, CancellationToken cancellationToken)
    {
        var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var parsedUserId))
        {
            throw new UnauthorizedAccessException("User not authenticated");
        }

        var demand = await _demandRepository.GetByIdAsync(request.Id);
        if (demand == null)
            throw new KeyNotFoundException($"Demand with id {request.Id} not found");

        if (demand.CreatedBy != parsedUserId)
            throw new UnauthorizedAccessException("You can only update your own demands");

        if (!string.IsNullOrEmpty(request.Title))
            demand.Title = request.Title;

        if (!string.IsNullOrEmpty(request.Description))
            demand.Description = request.Description;

        if (!string.IsNullOrEmpty(request.Category))
            demand.Category = request.Category;

        if (request.Subcategory != null)
            demand.Subcategory = request.Subcategory;

        if (request.Budget.HasValue)
            demand.Budget = request.Budget.Value;

        if (request.Photos != null)
            demand.Photos = request.Photos;

        if (request.Latitude.HasValue)
            demand.Latitude = request.Latitude.Value;

        if (request.Longitude.HasValue)
            demand.Longitude = request.Longitude.Value;

        if (request.Address != null)
            demand.Address = request.Address;

        if (request.ServiceRadiusKm.HasValue)
            demand.ServiceRadiusKm = request.ServiceRadiusKm.Value;

        demand.UpdatedAt = DateTime.UtcNow;

        await _demandRepository.UpdateAsync(demand);

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
