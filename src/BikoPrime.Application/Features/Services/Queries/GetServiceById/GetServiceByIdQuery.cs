namespace BikoPrime.Application.Features.Services.Queries.GetServiceById;

using MediatR;
using BikoPrime.Application.DTOs.Service;

public class GetServiceByIdQuery : IRequest<ServiceDto>
{
    public Guid Id { get; set; }
}
