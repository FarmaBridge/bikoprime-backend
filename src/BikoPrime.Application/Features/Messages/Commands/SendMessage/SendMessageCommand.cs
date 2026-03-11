namespace BikoPrime.Application.Features.Messages.Commands.SendMessage;

using MediatR;
using BikoPrime.Application.DTOs.Message;

public class SendMessageCommand : IRequest<MessageDto>
{
    public string ConversationId { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public string Type { get; set; } = "text";

    public string SenderId { get; set; } = string.Empty;
}
