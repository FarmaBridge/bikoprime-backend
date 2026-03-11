using MediatR;
using BikoPrime.Application.DTOs.Service;
using BikoPrime.Application.Interfaces;
using BikoPrime.Domain.Entities;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace BikoPrime.Application.Features.Services.Commands.CreateService;

public class CreateServiceCommandHandler : IRequestHandler<CreateServiceCommand, ServiceDto>
{
    private readonly IServiceRepository _serviceRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CreateServiceCommandHandler(IServiceRepository serviceRepository, IHttpContextAccessor httpContextAccessor)
    {
        _serviceRepository = serviceRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<ServiceDto> Handle(CreateServiceCommand request, CancellationToken cancellationToken)
    {
        var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var parsedUserId))
        {
            throw new UnauthorizedAccessException("User not authenticated");
        }

        var service = new Service
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            Category = request.Category,
            Subcategory = request.Subcategory,
            PriceType = request.PriceType,
            Price = request.Price,
            Photos = request.Photos,
            ProviderId = parsedUserId,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            Address = request.Address,
            ServiceRadiusKm = request.ServiceRadiusKm,
            Rating = 0,
            ReviewsCount = 0,
            CreatedAt = DateTime.UtcNow
        };

        await _serviceRepository.CreateAsync(service);

        return MapToDto(service);
    }

    private ServiceDto MapToDto(Service service)
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
