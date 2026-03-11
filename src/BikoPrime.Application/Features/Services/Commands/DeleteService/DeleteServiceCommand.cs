namespace BikoPrime.Application.Features.Services.Commands.DeleteService;

using MediatR;

public class DeleteServiceCommand : IRequest<Unit>
{
    public Guid Id { get; set; }

    public string? ProviderId { get; set; }
}
