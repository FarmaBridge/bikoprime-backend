namespace BikoPrime.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using BikoPrime.Application.Interfaces;
using BikoPrime.Domain.Entities;
using BikoPrime.Persistence.Data;

public class ReviewRepository : IReviewRepository
{
    private readonly BikoPrimeDbContext _context;

    public ReviewRepository(BikoPrimeDbContext context)
    {
        _context = context;
    }

    public async Task<Review?> GetByIdAsync(Guid id)
    {
        return await _context.Reviews
            .Include(r => r.Author)
            .Include(r => r.TargetUser)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<List<Review>> GetByTargetUserAsync(Guid targetUserId)
    {
        return await _context.Reviews
            .Where(r => r.TargetUserId == targetUserId)
            .Include(r => r.Author)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<Review>> GetByTargetUserIdAsync(Guid targetUserId)
    {
        return await GetByTargetUserAsync(targetUserId);
    }

    public async Task<List<Review>> GetByAuthorAsync(Guid authorId)
    {
        return await _context.Reviews
            .Where(r => r.AuthorId == authorId)
            .Include(r => r.TargetUser)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<Review>> GetByAuthorIdAsync(Guid authorId)
    {
        return await GetByAuthorAsync(authorId);
    }

    public async Task<Review?> GetByContractAndTargetUserAsync(Guid contractId, Guid targetUserId)
    {
        return await _context.Reviews
            .FirstOrDefaultAsync(r => r.ContractId == contractId && r.TargetUserId == targetUserId);
    }

    public async Task<List<Review>> GetByContractAsync(Guid contractId)
    {
        return await _context.Reviews
            .Where(r => r.ContractId == contractId)
            .Include(r => r.Author)
            .ToListAsync();
    }

    public async Task CreateAsync(Review review)
    {
        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Review review)
    {
        _context.Reviews.Update(review);
        await _context.SaveChangesAsync();
    }
}
