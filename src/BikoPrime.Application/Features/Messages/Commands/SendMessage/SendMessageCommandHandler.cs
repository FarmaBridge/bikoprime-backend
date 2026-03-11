using MediatR;
using BikoPrime.Application.DTOs.Message;
using BikoPrime.Application.Interfaces;
using BikoPrime.Domain.Entities;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace BikoPrime.Application.Features.Messages.Commands.SendMessage;

public class SendMessageCommandHandler : IRequestHandler<SendMessageCommand, MessageDto>
{
    private readonly IMessageRepository _messageRepository;
    private readonly IConversationRepository _conversationRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public SendMessageCommandHandler(IMessageRepository messageRepository, IConversationRepository conversationRepository, IHttpContextAccessor httpContextAccessor)
    {
        _messageRepository = messageRepository;
        _conversationRepository = conversationRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<MessageDto> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var parsedUserId))
        {
            throw new UnauthorizedAccessException("User not authenticated");
        }

        var conversation = await _conversationRepository.GetByIdAsync(Guid.Parse(request.ConversationId));
        if (conversation == null)
            throw new KeyNotFoundException($"Conversation with id {request.ConversationId} not found");

        if (!conversation.ParticipantIds.Contains(parsedUserId))
            throw new UnauthorizedAccessException("You are not a participant of this conversation");

        var message = new Message
        {
            Id = Guid.NewGuid(),
            ConversationId = Guid.Parse(request.ConversationId),
            SenderId = parsedUserId,
            Content = request.Content,
            Type = request.Type,
            CreatedAt = DateTime.UtcNow
        };

        await _messageRepository.CreateAsync(message);

        // Update conversation's last message
        conversation.LastMessageId = message.Id;
        conversation.UpdatedAt = DateTime.UtcNow;
        await _conversationRepository.UpdateAsync(conversation);

        return MapToDto(message);
    }

    private MessageDto MapToDto(Message message)
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
