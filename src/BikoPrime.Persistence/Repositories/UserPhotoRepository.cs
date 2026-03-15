namespace BikoPrime.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using BikoPrime.Application.Interfaces;
using BikoPrime.Domain.Entities;
using BikoPrime.Persistence.Data;

/// <summary>
/// Repositório para gerenciamento de fotos de usuário
/// Implementa padrão Clean Architecture (Application → Persistence)
/// </summary>
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

    public async Task<UserPhoto> UpdateAsync(UserPhoto userPhoto, CancellationToken cancellationToken)
    {
        _context.UserPhotos.Update(userPhoto);
        await SaveChangesAsync(cancellationToken);
        return userPhoto;
    }

    public async Task<UserPhoto?> GetByIdAsync(Guid photoId, CancellationToken cancellationToken)
    {
        // Força materialização completa de todas as colunas, incluindo PhotoData (byte[])
        // ToListAsync() garante que EF Core carrega todo o registro
        var list = await _context.UserPhotos
            .Where(p => p.Id == photoId)
            .ToListAsync(cancellationToken);
        
        return list.FirstOrDefault();
    }

    public async Task<UserPhoto?> GetLatestByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await _context.UserPhotos
            .Where(p => p.UserId == userId && p.IsActive)
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
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<UserPhoto?> GetByFileHashAsync(string fileHash, CancellationToken cancellationToken)
    {
        return await _context.UserPhotos
            .AsNoTracking()
            .Where(p => p.FileHash == fileHash && p.IsActive)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task DeactivateCurrentPhotoAsync(Guid userId, string deactivationReason, CancellationToken cancellationToken)
    {
        var currentPhotos = await _context.UserPhotos
            .Where(p => p.UserId == userId && p.IsActive)
            .ToListAsync(cancellationToken);

        foreach (var photo in currentPhotos)
        {
            photo.IsActive = false;
            photo.DeactivatedAt = DateTime.UtcNow;
            photo.DeactivationReason = deactivationReason;
        }

        if (currentPhotos.Any())
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
