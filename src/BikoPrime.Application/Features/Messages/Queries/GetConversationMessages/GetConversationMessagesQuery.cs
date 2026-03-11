namespace BikoPrime.Application.Features.Messages.Queries.GetConversationMessages;

using MediatR;
using BikoPrime.Application.DTOs.Message;

public class GetConversationMessagesQuery : IRequest<List<MessageDto>>
{
    public string ConversationId { get; set; } = string.Empty;

    public string UserId { get; set; } = string.Empty;

    public int PageNumber { get; set; } = 1;

    public int PageSize { get; set; } = 50;
}
