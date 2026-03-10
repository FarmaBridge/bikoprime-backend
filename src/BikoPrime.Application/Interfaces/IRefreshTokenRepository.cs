namespace BikoPrime.Application.Interfaces;

using BikoPrime.Domain.Entities;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetValidTokenAsync(string token);
    
    Task<RefreshToken?> GetByUserIdAsync(Guid userId);
    
    Task CreateAsync(RefreshToken refreshToken);
    
    Task RevokeAsync(string token);
    
    Task DeleteExpiredAsync();
}
