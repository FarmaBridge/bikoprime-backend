namespace BikoPrime.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using BikoPrime.Application.Interfaces;
using BikoPrime.Domain.Entities;
using BikoPrime.Persistence.Data;

public class ServiceRepository : IServiceRepository
{
    private readonly BikoPrimeDbContext _context;

    public ServiceRepository(BikoPrimeDbContext context)
    {
        _context = context;
    }

    public async Task<Service?> GetByIdAsync(Guid id)
    {
        return await _context.Services.FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<List<Service>> GetAllAsync()
    {
        return await _context.Services.OrderByDescending(s => s.CreatedAt).ToListAsync();
    }

    public async Task<List<Service>> GetByCategoryAsync(string category)
    {
        return await _context.Services
            .Where(s => s.Category == category)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<Service>> GetByProviderAsync(Guid providerId)
    {
        return await _context.Services
            .Where(s => s.ProviderId == providerId)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<Service>> SearchAsync(string category, string? subcategory)
    {
        var query = _context.Services.AsQueryable();

        if (!string.IsNullOrWhiteSpace(category))
            query = query.Where(s => s.Category == category);

        if (!string.IsNullOrWhiteSpace(subcategory))
            query = query.Where(s => s.Subcategory == subcategory);

        return await query.OrderByDescending(s => s.CreatedAt).ToListAsync();
    }

    public async Task<List<Service>> GetNearbyAsync(double latitude, double longitude, decimal radiusKm)
    {
        return await _context.Services
            .Where(s => s.Latitude >= latitude - (double)radiusKm / 111.0 &&
                        s.Latitude <= latitude + (double)radiusKm / 111.0 &&
                        s.Longitude >= longitude - (double)radiusKm / 111.0 &&
                        s.Longitude <= longitude + (double)radiusKm / 111.0)
            .OrderByDescending(s => s.Rating)
            .ToListAsync();
    }

    public async Task CreateAsync(Service service)
    {
        _context.Services.Add(service);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Service service)
    {
        service.UpdatedAt = DateTime.UtcNow;
        _context.Services.Update(service);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var service = await _context.Services.FirstOrDefaultAsync(s => s.Id == id);
        if (service != null)
        {
            _context.Services.Remove(service);
            await _context.SaveChangesAsync();
        }
    }
}
