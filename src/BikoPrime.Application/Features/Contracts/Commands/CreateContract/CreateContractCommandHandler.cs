using MediatR;
using BikoPrime.Application.DTOs.Contract;
using BikoPrime.Application.Interfaces;
using BikoPrime.Domain.Entities;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace BikoPrime.Application.Features.Contracts.Commands.CreateContract;

public class CreateContractCommandHandler : IRequestHandler<CreateContractCommand, ContractDto>
{
    private readonly IContractRepository _contractRepository;
    private readonly IServiceRepository _serviceRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CreateContractCommandHandler(IContractRepository contractRepository, IServiceRepository serviceRepository, IHttpContextAccessor httpContextAccessor)
    {
        _contractRepository = contractRepository;
        _serviceRepository = serviceRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<ContractDto> Handle(CreateContractCommand request, CancellationToken cancellationToken)
    {
        var service = await _serviceRepository.GetByIdAsync(request.ServiceId);
        if (service == null)
            throw new KeyNotFoundException($"Service with id {request.ServiceId} not found");

        var contract = new Contract
        {
            Id = Guid.NewGuid(),
            ServiceId = request.ServiceId,
            ProviderId = service.ProviderId,
            ClientId = request.ClientId,
            Status = "pending",
            CreatedAt = DateTime.UtcNow
        };

        await _contractRepository.CreateAsync(contract);

        return MapToDto(contract);
    }

    private ContractDto MapToDto(Contract contract)
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
