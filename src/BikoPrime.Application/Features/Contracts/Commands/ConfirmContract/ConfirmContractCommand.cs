namespace BikoPrime.Application.Features.Contracts.Commands.ConfirmContract;

using MediatR;
using BikoPrime.Application.DTOs.Contract;

public class ConfirmContractCommand : IRequest<ContractDto>
{
    public Guid Id { get; set; }

    public string? ContractId { get; set; }

    public string? UserId { get; set; }
}
