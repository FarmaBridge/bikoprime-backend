namespace BikoPrime.Application.Interfaces;

using BikoPrime.Domain.Entities;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    
    Task<User?> GetByUsernameAsync(string username);
    
    Task<User?> GetByIdAsync(Guid id);
    
    Task<bool> EmailExistsAsync(string email);
    
    Task<bool> UsernameExistsAsync(string username);
    
    Task CreateAsync(User user);
    
    Task UpdateAsync(User user);
}
