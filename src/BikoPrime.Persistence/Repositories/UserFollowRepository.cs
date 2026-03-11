namespace BikoPrime.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using BikoPrime.Application.Interfaces;
using BikoPrime.Domain.Entities;
using BikoPrime.Persistence.Data;

public class UserFollowRepository : IUserFollowRepository
{
    private readonly BikoPrimeDbContext _context;

    public UserFollowRepository(BikoPrimeDbContext context)
    {
        _context = context;
    }

    public async Task<UserFollow?> GetFollowAsync(Guid followerId, Guid followingId)
    {
        return await _context.UserFollows
            .FirstOrDefaultAsync(uf => uf.FollowerId == followerId && uf.FollowingId == followingId);
    }

    public async Task<List<UserFollow>> GetFollowersAsync(Guid userId)
    {
        return await _context.UserFollows
            .Where(uf => uf.FollowingId == userId)
            .Include(uf => uf.Follower)
            .ToListAsync();
    }

    public async Task<List<UserFollow>> GetFollowingAsync(Guid userId)
    {
        return await _context.UserFollows
            .Where(uf => uf.FollowerId == userId)
            .Include(uf => uf.Following)
            .ToListAsync();
    }

    public async Task CreateAsync(UserFollow userFollow)
    {
        _context.UserFollows.Add(userFollow);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var userFollow = await _context.UserFollows.FirstOrDefaultAsync(uf => uf.Id == id);
        if (userFollow != null)
        {
            _context.UserFollows.Remove(userFollow);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> IsFollowingAsync(Guid followerId, Guid followingId)
    {
        return await _context.UserFollows
            .AnyAsync(uf => uf.FollowerId == followerId && uf.FollowingId == followingId);
    }
}
