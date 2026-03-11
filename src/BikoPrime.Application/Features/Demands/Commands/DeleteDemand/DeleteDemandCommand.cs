namespace BikoPrime.Application.Features.Demands.Commands.DeleteDemand;

using MediatR;

public class DeleteDemandCommand : IRequest<Unit>
{
    public Guid Id { get; set; }

    public string? UserId { get; set; }
}
