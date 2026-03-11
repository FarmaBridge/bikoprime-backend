namespace BikoPrime.Application.Features.Services.Queries.GetServicesByCategory;

using MediatR;
using BikoPrime.Application.DTOs.Service;

public class GetServicesByCategoryQuery : IRequest<List<ServiceDto>>
{
    public string Category { get; set; } = string.Empty;
}
