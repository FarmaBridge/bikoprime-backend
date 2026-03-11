namespace BikoPrime.Application.Interfaces;

using BikoPrime.Domain.Entities;

public interface IConversationRepository
{
    Task<Conversation?> GetByIdAsync(Guid id);
    
    Task<List<Conversation>> GetByUserAsync(Guid userId);
    
    Task<List<Conversation>> GetByUserIdAsync(Guid userId);
    
    Task<Conversation?> GetByParticipantsAsync(Guid userId1, Guid userId2);
    
    Task CreateAsync(Conversation conversation);
    
    Task UpdateAsync(Conversation conversation);
}
