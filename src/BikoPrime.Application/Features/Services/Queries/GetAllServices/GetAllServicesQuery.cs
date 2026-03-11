namespace BikoPrime.Application.Features.Services.Queries.GetAllServices;

using MediatR;
using BikoPrime.Application.DTOs.Service;

public class GetAllServicesQuery : IRequest<List<ServiceDto>>
{
}
