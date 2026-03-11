using MediatR;
using BikoPrime.Application.DTOs.Message;
using BikoPrime.Application.Interfaces;
using BikoPrime.Domain.Entities;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace BikoPrime.Application.Features.Messages.Commands.CreateConversation;

public class CreateConversationCommandHandler : IRequestHandler<CreateConversationCommand, ConversationDto>
{
    private readonly IConversationRepository _conversationRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CreateConversationCommandHandler(IConversationRepository conversationRepository, IHttpContextAccessor httpContextAccessor)
    {
        _conversationRepository = conversationRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<ConversationDto> Handle(CreateConversationCommand request, CancellationToken cancellationToken)
    {
        var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var parsedUserId))
        {
            throw new UnauthorizedAccessException("User not authenticated");
        }

        // Ensure current user is in participants
        if (!request.ParticipantIds.Contains(parsedUserId))
        {
            request.ParticipantIds.Add(parsedUserId);
        }

        var conversation = new Conversation
        {
            Id = Guid.NewGuid(),
            ParticipantIds = request.ParticipantIds,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _conversationRepository.CreateAsync(conversation);

        return MapToDto(conversation);
    }

    private ConversationDto MapToDto(Conversation conversation)
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
