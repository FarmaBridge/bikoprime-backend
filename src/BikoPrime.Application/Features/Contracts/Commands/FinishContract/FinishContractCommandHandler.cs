using MediatR;
using BikoPrime.Application.DTOs.Contract;
using BikoPrime.Application.Interfaces;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace BikoPrime.Application.Features.Contracts.Commands.FinishContract;

public class FinishContractCommandHandler : IRequestHandler<FinishContractCommand, ContractDto>
{
    private readonly IContractRepository _contractRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public FinishContractCommandHandler(IContractRepository contractRepository, IHttpContextAccessor httpContextAccessor)
    {
        _contractRepository = contractRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<ContractDto> Handle(FinishContractCommand request, CancellationToken cancellationToken)
    {
        var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var parsedUserId))
        {
            throw new UnauthorizedAccessException("User not authenticated");
        }

        var contract = await _contractRepository.GetByIdAsync(request.Id);
        if (contract == null)
            throw new KeyNotFoundException($"Contract with id {request.Id} not found");

        // Only provider or client can finish
        if (contract.ProviderId != parsedUserId && contract.ClientId != parsedUserId)
            throw new UnauthorizedAccessException("You are not authorized to finish this contract");

        if (contract.Status != "confirmed")
            throw new InvalidOperationException($"Cannot finish contract in {contract.Status} status");

        contract.Status = "finished";
        contract.FinishedAt = DateTime.UtcNow;
        await _contractRepository.UpdateAsync(contract);

        return MapToDto(contract);
    }

    private ContractDto MapToDto(BikoPrime.Domain.Entities.Contract contract)
    {
        return new ContractDto
        {
            Id = contract.Id,
            ServiceId = contract.ServiceId,
            ProviderId = contract.ProviderId,
            ClientId = contract.ClientId,
            Status = contract.Status,
            CreatedAt = contract.CreatedAt,
            FinishedAt = contract.FinishedAt
        };
    }
}
