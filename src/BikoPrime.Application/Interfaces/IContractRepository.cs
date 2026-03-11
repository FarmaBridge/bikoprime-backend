namespace BikoPrime.Application.Interfaces;

using BikoPrime.Domain.Entities;

public interface IContractRepository
{
    Task<Contract?> GetByIdAsync(Guid id);
    
    Task<List<Contract>> GetByUserAsync(Guid userId);
    
    Task<List<Contract>> GetByProviderAsync(Guid providerId);
    
        Task<List<Contract>> GetByProviderIdAsync(Guid providerId);
        
        Task<List<Contract>> GetByClientAsync(Guid clientId);
        
        Task<List<Contract>> GetByClientIdAsync(Guid clientId);
    
    Task<List<Contract>> GetByServiceAsync(Guid serviceId);
    
    Task CreateAsync(Contract contract);
    
    Task UpdateAsync(Contract contract);
}
