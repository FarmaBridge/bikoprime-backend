namespace BikoPrime.Application.Interfaces;

using BikoPrime.Domain.Entities;

public interface IServiceRepository
{
    Task<Service?> GetByIdAsync(Guid id);
    
    Task<List<Service>> GetAllAsync();
    
    Task<List<Service>> GetByCategoryAsync(string category);
    
    Task<List<Service>> GetByProviderAsync(Guid providerId);
    
    Task<List<Service>> SearchAsync(string category, string? subcategory);
    
    Task<List<Service>> GetNearbyAsync(double latitude, double longitude, decimal radiusKm);
    
    Task CreateAsync(Service service);
    
    Task UpdateAsync(Service service);
    
    Task DeleteAsync(Guid id);
}
