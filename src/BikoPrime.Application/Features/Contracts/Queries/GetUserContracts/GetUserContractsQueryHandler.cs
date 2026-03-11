using MediatR;
using BikoPrime.Application.DTOs.Contract;
using BikoPrime.Application.Interfaces;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace BikoPrime.Application.Features.Contracts.Queries.GetUserContracts;

public class GetUserContractsQueryHandler : IRequestHandler<GetUserContractsQuery, List<ContractDto>>
{
    private readonly IContractRepository _contractRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetUserContractsQueryHandler(IContractRepository contractRepository, IHttpContextAccessor httpContextAccessor)
    {
        _contractRepository = contractRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<List<ContractDto>> Handle(GetUserContractsQuery request, CancellationToken cancellationToken)
    {
        var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var parsedUserId))
        {
            throw new UnauthorizedAccessException("User not authenticated");
        }

        var contracts = await _contractRepository.GetByClientIdAsync(parsedUserId);
        return contracts.Select(MapToDto).ToList();
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
