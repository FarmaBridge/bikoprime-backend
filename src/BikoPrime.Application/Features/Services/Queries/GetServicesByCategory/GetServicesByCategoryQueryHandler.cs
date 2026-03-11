namespace BikoPrime.Application.Features.Services.Queries.GetServicesByCategory;

using MediatR;
using BikoPrime.Application.DTOs.Service;
using BikoPrime.Application.Interfaces;
using BikoPrime.Application.DTOs.Auth;

public class GetServicesByCategoryQueryHandler : IRequestHandler<GetServicesByCategoryQuery, List<ServiceDto>>
{
    private readonly IServiceRepository _serviceRepository;

    public GetServicesByCategoryQueryHandler(IServiceRepository serviceRepository)
    {
        _serviceRepository = serviceRepository;
    }

    public async Task<List<ServiceDto>> Handle(GetServicesByCategoryQuery request, CancellationToken cancellationToken)
    {
        var services = await _serviceRepository.GetByCategoryAsync(request.Category);
        return services.Select(MapToDto).ToList();
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
