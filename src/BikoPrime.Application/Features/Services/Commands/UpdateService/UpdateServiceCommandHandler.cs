using MediatR;
using BikoPrime.Application.DTOs.Service;
using BikoPrime.Application.Interfaces;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace BikoPrime.Application.Features.Services.Commands.UpdateService;

public class UpdateServiceCommandHandler : IRequestHandler<UpdateServiceCommand, ServiceDto>
{
    private readonly IServiceRepository _serviceRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UpdateServiceCommandHandler(IServiceRepository serviceRepository, IHttpContextAccessor httpContextAccessor)
    {
        _serviceRepository = serviceRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<ServiceDto> Handle(UpdateServiceCommand request, CancellationToken cancellationToken)
    {
        var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var parsedUserId))
        {
            throw new UnauthorizedAccessException("User not authenticated");
        }

        var service = await _serviceRepository.GetByIdAsync(request.Id);
        if (service == null)
            throw new KeyNotFoundException($"Service with id {request.Id} not found");

        if (service.ProviderId != parsedUserId)
            throw new UnauthorizedAccessException("You can only update your own services");

        if (!string.IsNullOrEmpty(request.Title))
            service.Title = request.Title;

        if (!string.IsNullOrEmpty(request.Description))
            service.Description = request.Description;

        if (!string.IsNullOrEmpty(request.Category))
            service.Category = request.Category;

        if (request.Subcategory != null)
            service.Subcategory = request.Subcategory;

        if (!string.IsNullOrEmpty(request.PriceType))
            service.PriceType = request.PriceType;

        if (request.Price.HasValue)
            service.Price = request.Price.Value;

        if (request.Photos != null)
            service.Photos = request.Photos;

        if (request.Latitude.HasValue)
            service.Latitude = request.Latitude.Value;

        if (request.Longitude.HasValue)
            service.Longitude = request.Longitude.Value;

        if (request.Address != null)
            service.Address = request.Address;

        if (request.ServiceRadiusKm.HasValue)
            service.ServiceRadiusKm = request.ServiceRadiusKm.Value;

        await _serviceRepository.UpdateAsync(service);

        return MapToDto(service);
    }

    private ServiceDto MapToDto(BikoPrime.Domain.Entities.Service service)
    {
        return new ServiceDto
        {
            Id = service.Id,
            Title = service.Title,
            Description = service.Description,
            Category = service.Category,
            Subcategory = service.Subcategory,
            PriceType = service.PriceType,
            Price = service.Price,
            Photos = service.Photos,
            ProviderId = service.ProviderId,
            Location = new BikoPrime.Application.DTOs.Auth.LocationDto
            {
                Latitude = service.Latitude,
                Longitude = service.Longitude,
                Address = service.Address
            },
            ServiceRadiusKm = service.ServiceRadiusKm,
            Rating = service.Rating,
            ReviewsCount = service.ReviewsCount,
            CreatedAt = service.CreatedAt,
            UpdatedAt = service.UpdatedAt
        };
    }
}
