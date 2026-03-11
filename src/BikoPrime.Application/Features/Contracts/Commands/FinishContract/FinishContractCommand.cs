namespace BikoPrime.Application.Features.Contracts.Commands.FinishContract;

using MediatR;
using BikoPrime.Application.DTOs.Contract;

public class FinishContractCommand : IRequest<ContractDto>
{
    public Guid Id { get; set; }

    public string? ContractId { get; set; }

    public string? UserId { get; set; }
}
