namespace BikoPrime.Application.Features.Messages.Queries.GetConversations;

using MediatR;
using BikoPrime.Application.DTOs.Message;

public class GetConversationsQuery : IRequest<List<ConversationDto>>
{
    public string UserId { get; set; } = string.Empty;
}
