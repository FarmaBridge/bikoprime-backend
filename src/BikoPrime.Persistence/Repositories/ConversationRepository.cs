namespace BikoPrime.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using BikoPrime.Application.Interfaces;
using BikoPrime.Domain.Entities;
using BikoPrime.Persistence.Data;

public class ConversationRepository : IConversationRepository
{
    private readonly BikoPrimeDbContext _context;

    public ConversationRepository(BikoPrimeDbContext context)
    {
        _context = context;
    }

    public async Task<Conversation?> GetByIdAsync(Guid id)
    {
        return await _context.Conversations
            .Include(c => c.Messages)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<List<Conversation>> GetByUserAsync(Guid userId)
    {
        return await _context.Conversations
            .Where(c => c.ParticipantIds.Contains(userId))
            .Include(c => c.LastMessage)
            .OrderByDescending(c => c.UpdatedAt)
            .ToListAsync();
    }

    public async Task<List<Conversation>> GetByUserIdAsync(Guid userId)
    {
        return await GetByUserAsync(userId);
    }

    public async Task<Conversation?> GetByParticipantsAsync(Guid userId1, Guid userId2)
    {
        return await _context.Conversations
            .FirstOrDefaultAsync(c => 
                (c.ParticipantIds.Contains(userId1) && c.ParticipantIds.Contains(userId2)) &&
                c.ParticipantIds.Count == 2);
    }

    public async Task CreateAsync(Conversation conversation)
    {
        _context.Conversations.Add(conversation);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Conversation conversation)
    {
        conversation.UpdatedAt = DateTime.UtcNow;
        _context.Conversations.Update(conversation);
        await _context.SaveChangesAsync();
    }
}
