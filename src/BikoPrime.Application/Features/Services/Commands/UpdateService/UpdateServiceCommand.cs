namespace BikoPrime.Application.Features.Services.Commands.UpdateService;

using MediatR;
using BikoPrime.Application.DTOs.Service;

public class UpdateServiceCommand : IRequest<ServiceDto>
{
    public Guid Id { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public string? Category { get; set; }

    public string? Subcategory { get; set; }

    public string? PriceType { get; set; }

    public decimal? Price { get; set; }

    public List<string>? Photos { get; set; }

    public double? Latitude { get; set; }

    public double? Longitude { get; set; }

    public string? Address { get; set; }

    public decimal? ServiceRadiusKm { get; set; }

    public string? ProviderId { get; set; }
}
