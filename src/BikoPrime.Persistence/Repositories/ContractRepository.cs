namespace BikoPrime.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using BikoPrime.Application.Interfaces;
using BikoPrime.Domain.Entities;
using BikoPrime.Persistence.Data;

public class ContractRepository : IContractRepository
{
    private readonly BikoPrimeDbContext _context;

    public ContractRepository(BikoPrimeDbContext context)
    {
        _context = context;
    }

    public async Task<Contract?> GetByIdAsync(Guid id)
    {
        return await _context.Contracts
            .Include(c => c.Service)
            .Include(c => c.Provider)
            .Include(c => c.Client)
            .Include(c => c.Reviews)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<List<Contract>> GetByUserAsync(Guid userId)
    {
        return await _context.Contracts
            .Where(c => c.ProviderId == userId || c.ClientId == userId)
            .Include(c => c.Service)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<Contract>> GetByProviderAsync(Guid providerId)
    {
        return await _context.Contracts
            .Where(c => c.ProviderId == providerId)
            .Include(c => c.Service)
            .Include(c => c.Client)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<Contract>> GetByClientAsync(Guid clientId)
    {
        return await _context.Contracts
            .Where(c => c.ClientId == clientId)
            .Include(c => c.Service)
            .Include(c => c.Provider)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<Contract>> GetByClientIdAsync(Guid clientId)
    {
        return await GetByClientAsync(clientId);
    }

    public async Task<List<Contract>> GetByProviderIdAsync(Guid providerId)
    {
        return await GetByProviderAsync(providerId);
    }

    public async Task<List<Contract>> GetByServiceAsync(Guid serviceId)
    {
        return await _context.Contracts
            .Where(c => c.ServiceId == serviceId)
            .Include(c => c.Client)
            .ToListAsync();
    }

    public async Task CreateAsync(Contract contract)
    {
        _context.Contracts.Add(contract);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Contract contract)
    {
        _context.Contracts.Update(contract);
        await _context.SaveChangesAsync();
    }
}
