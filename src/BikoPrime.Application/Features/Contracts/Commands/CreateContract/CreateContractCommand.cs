namespace BikoPrime.Application.Features.Contracts.Commands.CreateContract;

using MediatR;
using BikoPrime.Application.DTOs.Contract;

public class CreateContractCommand : IRequest<ContractDto>
{
    public Guid ServiceId { get; set; }

    public Guid ClientId { get; set; }

    public string InitiatorId { get; set; } = string.Empty;
}
