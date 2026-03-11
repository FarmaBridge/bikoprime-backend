namespace BikoPrime.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using BikoPrime.Application.Interfaces;
using BikoPrime.Domain.Entities;
using BikoPrime.Persistence.Data;

public class MessageRepository : IMessageRepository
{
    private readonly BikoPrimeDbContext _context;

    public MessageRepository(BikoPrimeDbContext context)
    {
        _context = context;
    }

    public async Task<Message?> GetByIdAsync(Guid id)
    {
        return await _context.Messages.FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<List<Message>> GetByConversationAsync(Guid conversationId)
    {
        return await _context.Messages
            .Where(m => m.ConversationId == conversationId)
            .OrderBy(m => m.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<Message>> GetByConversationIdAsync(Guid conversationId)
    {
        return await GetByConversationAsync(conversationId);
    }

    public async Task CreateAsync(Message message)
    {
        _context.Messages.Add(message);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var message = await _context.Messages.FirstOrDefaultAsync(m => m.Id == id);
        if (message != null)
        {
            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();
        }
    }
}
