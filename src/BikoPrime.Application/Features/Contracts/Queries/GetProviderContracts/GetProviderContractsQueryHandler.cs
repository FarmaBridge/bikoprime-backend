namespace BikoPrime.Application.Features.Contracts.Queries.GetProviderContracts;

using MediatR;
using BikoPrime.Application.DTOs.Contract;
using BikoPrime.Application.Interfaces;

public class GetProviderContractsQueryHandler : IRequestHandler<GetProviderContractsQuery, List<ContractDto>>
{
    private readonly IContractRepository _contractRepository;

    public GetProviderContractsQueryHandler(IContractRepository contractRepository)
    {
        _contractRepository = contractRepository;
    }

    public async Task<List<ContractDto>> Handle(GetProviderContractsQuery request, CancellationToken cancellationToken)
    {
        var contracts = await _contractRepository.GetByProviderIdAsync(request.ProviderId);
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
