namespace BikoPrime.Application.Features.Services.Queries.GetServiceById;

using MediatR;
using BikoPrime.Application.DTOs.Service;
using BikoPrime.Application.Interfaces;
using BikoPrime.Application.DTOs.Auth;

public class GetServiceByIdQueryHandler : IRequestHandler<GetServiceByIdQuery, ServiceDto>
{
    private readonly IServiceRepository _serviceRepository;

    public GetServiceByIdQueryHandler(IServiceRepository serviceRepository)
    {
        _serviceRepository = serviceRepository;
    }

    public async Task<ServiceDto> Handle(GetServiceByIdQuery request, CancellationToken cancellationToken)
    {
        var service = await _serviceRepository.GetByIdAsync(request.Id);
        if (service == null)
            throw new KeyNotFoundException($"Service with id {request.Id} not found");

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
            Location = new LocationDto
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
