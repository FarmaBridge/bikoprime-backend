namespace BikoPrime.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using BikoPrime.Application.Interfaces;
using BikoPrime.Domain.Entities;
using BikoPrime.Persistence.Data;

public class DemandRepository : IDemandRepository
{
    private readonly BikoPrimeDbContext _context;

    public DemandRepository(BikoPrimeDbContext context)
    {
        _context = context;
    }

    public async Task<Demand?> GetByIdAsync(Guid id)
    {
        return await _context.Demands.FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<List<Demand>> GetAllAsync()
    {
        return await _context.Demands.OrderByDescending(d => d.CreatedAt).ToListAsync();
    }

    public async Task<List<Demand>> GetByCategoryAsync(string category)
    {
        return await _context.Demands
            .Where(d => d.Category == category)
            .OrderByDescending(d => d.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<Demand>> GetByUserAsync(Guid userId)
    {
        return await _context.Demands
            .Where(d => d.CreatedBy == userId)
            .OrderByDescending(d => d.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<Demand>> GetByUserIdAsync(Guid userId)
    {
        return await GetByUserAsync(userId);
    }

    public async Task<List<Demand>> SearchAsync(string category, string? subcategory)
    {
        var query = _context.Demands.AsQueryable();

        if (!string.IsNullOrWhiteSpace(category))
            query = query.Where(d => d.Category == category);

        if (!string.IsNullOrWhiteSpace(subcategory))
            query = query.Where(d => d.Subcategory == subcategory);

        return await query.OrderByDescending(d => d.CreatedAt).ToListAsync();
    }

    public async Task<List<Demand>> GetNearbyAsync(double latitude, double longitude, decimal radiusKm)
    {
        return await _context.Demands
            .Where(d => d.Latitude >= latitude - (double)radiusKm / 111.0 &&
                        d.Latitude <= latitude + (double)radiusKm / 111.0 &&
                        d.Longitude >= longitude - (double)radiusKm / 111.0 &&
                        d.Longitude <= longitude + (double)radiusKm / 111.0)
            .OrderByDescending(d => d.CreatedAt)
            .ToListAsync();
    }

    public async Task CreateAsync(Demand demand)
    {
        _context.Demands.Add(demand);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Demand demand)
    {
        demand.UpdatedAt = DateTime.UtcNow;
        _context.Demands.Update(demand);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var demand = await _context.Demands.FirstOrDefaultAsync(d => d.Id == id);
        if (demand != null)
        {
            _context.Demands.Remove(demand);
            await _context.SaveChangesAsync();
        }
    }
}
