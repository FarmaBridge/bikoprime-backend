namespace BikoPrime.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using BikoPrime.Application.Interfaces;
using BikoPrime.Domain.Entities;
using BikoPrime.Persistence.Data;

public class UserRepository : IUserRepository
{
    private readonly BikoPrimeDbContext _context;

    public UserRepository(BikoPrimeDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _context.Users.AnyAsync(u => u.Email == email);
    }

    public async Task<bool> UsernameExistsAsync(string username)
    {
        return await _context.Users.AnyAsync(u => u.UserName == username);
    }

    public async Task CreateAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task<List<User>> SearchAsync(string query)
    {
        return await _context.Users
            .Where(u => u.UserName.Contains(query) || u.Email.Contains(query))
            .OrderBy(u => u.UserName)
            .ToListAsync();
    }

    public async Task<List<User>> GetByIdsAsync(List<Guid> ids)
    {
        return await _context.Users
            .Where(u => ids.Contains(u.Id))
            .ToListAsync();
    }

    public async Task<List<User>> GetRecommendedAsync()
    {
        return await _context.Users
            .OrderByDescending(u => u.CreatedAt)
            .Take(10)
            .ToListAsync();
    }
}
