namespace BikoPrime.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using BikoPrime.Application.Interfaces;
using BikoPrime.Domain.Entities;
using BikoPrime.Persistence.Data;

public class UserPhotoRepository : IUserPhotoRepository
{
    private readonly BikoPrimeDbContext _context;

    public UserPhotoRepository(BikoPrimeDbContext context)
    {
        _context = context;
    }

    public async Task<UserPhoto> AddAsync(UserPhoto userPhoto, CancellationToken cancellationToken)
    {
        await _context.UserPhotos.AddAsync(userPhoto, cancellationToken);
        return userPhoto;
    }

    public async Task<UserPhoto?> GetByIdAsync(Guid photoId, CancellationToken cancellationToken)
    {
        return await _context.UserPhotos
            .FirstOrDefaultAsync(p => p.Id == photoId, cancellationToken);
    }

    public async Task<UserPhoto?> GetLatestByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await _context.UserPhotos
            .Where(p => p.UserId == userId && p.IsProfilePicture)
            .OrderByDescending(p => p.UploadedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<UserPhoto>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await _context.UserPhotos
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.UploadedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid photoId, CancellationToken cancellationToken)
    {
        var photo = await GetByIdAsync(photoId, cancellationToken);
        if (photo != null)
        {
            _context.UserPhotos.Remove(photo);
        }

        await Task.CompletedTask;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
