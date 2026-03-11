using MediatR;
using BikoPrime.Application.Interfaces;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace BikoPrime.Application.Features.Services.Commands.DeleteService;

public class DeleteServiceCommandHandler : IRequestHandler<DeleteServiceCommand, Unit>
{
    private readonly IServiceRepository _serviceRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public DeleteServiceCommandHandler(IServiceRepository serviceRepository, IHttpContextAccessor httpContextAccessor)
    {
        _serviceRepository = serviceRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Unit> Handle(DeleteServiceCommand request, CancellationToken cancellationToken)
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
            throw new UnauthorizedAccessException("You can only delete your own services");

        await _serviceRepository.DeleteAsync(request.Id);

        return Unit.Value;
    }
}
