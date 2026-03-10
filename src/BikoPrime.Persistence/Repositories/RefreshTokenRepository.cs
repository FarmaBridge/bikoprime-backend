namespace BikoPrime.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using BikoPrime.Application.Interfaces;
using BikoPrime.Domain.Entities;
using BikoPrime.Persistence.Data;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly BikoPrimeDbContext _context;

    public RefreshTokenRepository(BikoPrimeDbContext context)
    {
        _context = context;
    }

    public async Task<RefreshToken?> GetValidTokenAsync(string token)
    {
        return await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == token && !rt.IsRevoked && rt.ExpiresAt > DateTime.UtcNow);
    }

    public async Task<RefreshToken?> GetByUserIdAsync(Guid userId)
    {
        return await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.UserId == userId && !rt.IsRevoked && rt.ExpiresAt > DateTime.UtcNow);
    }

    public async Task CreateAsync(RefreshToken refreshToken)
    {
        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();
    }

    public async Task RevokeAsync(string token)
    {
        var refreshToken = await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == token);
        if (refreshToken != null)
        {
            refreshToken.IsRevoked = true;
            _context.RefreshTokens.Update(refreshToken);
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteExpiredAsync()
    {
        var expiredTokens = await _context.RefreshTokens
            .Where(rt => rt.ExpiresAt <= DateTime.UtcNow)
            .ToListAsync();

        if (expiredTokens.Count > 0)
        {
            _context.RefreshTokens.RemoveRange(expiredTokens);
            await _context.SaveChangesAsync();
        }
    }
}
