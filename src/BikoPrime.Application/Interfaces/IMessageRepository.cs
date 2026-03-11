namespace BikoPrime.Application.Interfaces;

using BikoPrime.Domain.Entities;

public interface IMessageRepository
{
    Task<Message?> GetByIdAsync(Guid id);
    
    Task<List<Message>> GetByConversationAsync(Guid conversationId);
        
        Task<List<Message>> GetByConversationIdAsync(Guid conversationId);
    
    Task CreateAsync(Message message);
    
    Task DeleteAsync(Guid id);
}
