namespace BikoPrime.Application.Features.Contracts.Queries.GetProviderContracts;

using MediatR;
using BikoPrime.Application.DTOs.Contract;

public class GetProviderContractsQuery : IRequest<List<ContractDto>>
{
    public Guid ProviderId { get; set; }
}
