namespace BikoPrime.Application.Interfaces;

using BikoPrime.Domain.Entities;

public interface IUserFollowRepository
{
    Task<UserFollow?> GetFollowAsync(Guid followerId, Guid followingId);
    
    Task<List<UserFollow>> GetFollowersAsync(Guid userId);
    
    Task<List<UserFollow>> GetFollowingAsync(Guid userId);
        
        Task<bool> IsFollowingAsync(Guid followerId, Guid followingId);
    
    Task CreateAsync(UserFollow userFollow);
    
    Task DeleteAsync(Guid id);
}
