namespace BikoPrime.Application.Features.Contracts.Queries.GetContractById;

using MediatR;
using BikoPrime.Application.DTOs.Contract;

public class GetContractByIdQuery : IRequest<ContractDto>
{
    public Guid Id { get; set; }

    public string? ContractId { get; set; }

    public string? UserId { get; set; }
}
