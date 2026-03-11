namespace BikoPrime.Application.Features.Contracts.Queries.GetUserContracts;

using MediatR;
using BikoPrime.Application.DTOs.Contract;

public class GetUserContractsQuery : IRequest<List<ContractDto>>
{
    public string? UserId { get; set; }

    public string? Status { get; set; }

    public int PageNumber { get; set; }

    public int PageSize { get; set; }
}
