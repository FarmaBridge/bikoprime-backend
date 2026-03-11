using MediatR;
using BikoPrime.Application.DTOs.Message;
using BikoPrime.Application.Interfaces;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace BikoPrime.Application.Features.Messages.Queries.GetConversations;

public class GetConversationsQueryHandler : IRequestHandler<GetConversationsQuery, List<ConversationDto>>
{
    private readonly IConversationRepository _conversationRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetConversationsQueryHandler(IConversationRepository conversationRepository, IHttpContextAccessor httpContextAccessor)
    {
        _conversationRepository = conversationRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<List<ConversationDto>> Handle(GetConversationsQuery request, CancellationToken cancellationToken)
    {
        var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var parsedUserId))
        {
            throw new UnauthorizedAccessException("User not authenticated");
        }

        var conversations = await _conversationRepository.GetByUserIdAsync(parsedUserId);
        return conversations.Select(MapToDto).ToList();
    }

    private ConversationDto MapToDto(BikoPrime.Domain.Entities.Conversation conversation)
    {
        return new ConversationDto
        {
            Id = conversation.Id,
            ParticipantIds = conversation.ParticipantIds,
            LastMessage = conversation.LastMessage != null ? new MessageDto
            {
                Id = conversation.LastMessage.Id,
                ConversationId = conversation.LastMessage.ConversationId,
                SenderId = conversation.LastMessage.SenderId,
                Content = conversation.LastMessage.Content,
                Type = conversation.LastMessage.Type,
                CreatedAt = conversation.LastMessage.CreatedAt
            } : null,
            CreatedAt = conversation.CreatedAt,
            UpdatedAt = conversation.UpdatedAt
        };
    }
}
