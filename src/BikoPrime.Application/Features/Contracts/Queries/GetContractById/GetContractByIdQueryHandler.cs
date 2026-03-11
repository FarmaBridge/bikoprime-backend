namespace BikoPrime.Application.Features.Contracts.Queries.GetContractById;

using MediatR;
using BikoPrime.Application.DTOs.Contract;
using BikoPrime.Application.Interfaces;

public class GetContractByIdQueryHandler : IRequestHandler<GetContractByIdQuery, ContractDto>
{
    private readonly IContractRepository _contractRepository;

    public GetContractByIdQueryHandler(IContractRepository contractRepository)
    {
        _contractRepository = contractRepository;
    }

    public async Task<ContractDto> Handle(GetContractByIdQuery request, CancellationToken cancellationToken)
    {
        var contract = await _contractRepository.GetByIdAsync(request.Id);
        if (contract == null)
            throw new KeyNotFoundException($"Contract with id {request.Id} not found");

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
