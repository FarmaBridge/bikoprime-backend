namespace BikoPrime.Application.Interfaces;

using BikoPrime.Domain.Entities;

public interface IDemandRepository
{
    Task<Demand?> GetByIdAsync(Guid id);
    
    Task<List<Demand>> GetAllAsync();
    
    Task<List<Demand>> GetByCategoryAsync(string category);
    
    Task<List<Demand>> GetByUserAsync(Guid userId);
        
        Task<List<Demand>> GetByUserIdAsync(Guid userId);
    
    Task<List<Demand>> SearchAsync(string category, string? subcategory);
    
    Task<List<Demand>> GetNearbyAsync(double latitude, double longitude, decimal radiusKm);
    
    Task CreateAsync(Demand demand);
    
    Task UpdateAsync(Demand demand);
    
    Task DeleteAsync(Guid id);
}
