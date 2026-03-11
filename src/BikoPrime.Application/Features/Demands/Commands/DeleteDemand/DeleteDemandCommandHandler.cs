using MediatR;
using BikoPrime.Application.Interfaces;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace BikoPrime.Application.Features.Demands.Commands.DeleteDemand;

public class DeleteDemandCommandHandler : IRequestHandler<DeleteDemandCommand, Unit>
{
    private readonly IDemandRepository _demandRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public DeleteDemandCommandHandler(IDemandRepository demandRepository, IHttpContextAccessor httpContextAccessor)
    {
        _demandRepository = demandRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Unit> Handle(DeleteDemandCommand request, CancellationToken cancellationToken)
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
            throw new UnauthorizedAccessException("You can only delete your own demands");

        await _demandRepository.DeleteAsync(request.Id);

        return Unit.Value;
    }
}
