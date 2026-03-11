namespace BikoPrime.Application.Interfaces;

using BikoPrime.Domain.Entities;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    
    Task<User?> GetByUsernameAsync(string username);
    
    Task<User?> GetByIdAsync(Guid id);
    
    Task<bool> EmailExistsAsync(string email);
    
    Task<bool> UsernameExistsAsync(string username);
        
        Task<List<User>> SearchAsync(string query);
        
        Task<List<User>> GetByIdsAsync(List<Guid> ids);
        
        Task<List<User>> GetRecommendedAsync();
    
    Task CreateAsync(User user);
    
    Task UpdateAsync(User user);
}
