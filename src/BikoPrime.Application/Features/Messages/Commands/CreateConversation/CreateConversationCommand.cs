namespace BikoPrime.Application.Features.Messages.Commands.CreateConversation;

using MediatR;
using BikoPrime.Application.DTOs.Message;

public class CreateConversationCommand : IRequest<ConversationDto>
{
    public List<Guid> ParticipantIds { get; set; } = new List<Guid>();

    public string InitiatorId { get; set; } = string.Empty;
}
