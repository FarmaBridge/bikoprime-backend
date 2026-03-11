namespace BikoPrime.Application.Features.Messages.Queries.GetConversationMessages;

using MediatR;
using BikoPrime.Application.DTOs.Message;
using BikoPrime.Application.Interfaces;

public class GetConversationMessagesQueryHandler : IRequestHandler<GetConversationMessagesQuery, List<MessageDto>>
{
    private readonly IMessageRepository _messageRepository;

    public GetConversationMessagesQueryHandler(IMessageRepository messageRepository)
    {
        _messageRepository = messageRepository;
    }

    public async Task<List<MessageDto>> Handle(GetConversationMessagesQuery request, CancellationToken cancellationToken)
    {
        var messages = await _messageRepository.GetByConversationIdAsync(Guid.Parse(request.ConversationId));
        return messages.Select(MapToDto).ToList();
    }

    private MessageDto MapToDto(BikoPrime.Domain.Entities.Message message)
    {
        return new MessageDto
        {
            Id = message.Id,
            ConversationId = message.ConversationId,
            SenderId = message.SenderId,
            Content = message.Content,
            Type = message.Type,
            CreatedAt = message.CreatedAt
        };
    }
}
