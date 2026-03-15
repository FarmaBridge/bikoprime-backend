namespace BikoPrime.Application.Interfaces;

using BikoPrime.Domain.Entities;

public interface IUserPhotoRepository
{
    Task<UserPhoto> AddAsync(UserPhoto userPhoto, CancellationToken cancellationToken);

    Task<UserPhoto> UpdateAsync(UserPhoto userPhoto, CancellationToken cancellationToken);

    Task<UserPhoto?> GetByIdAsync(Guid photoId, CancellationToken cancellationToken);

    Task<UserPhoto?> GetLatestByUserIdAsync(Guid userId, CancellationToken cancellationToken);

    Task<List<UserPhoto>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);

    Task DeleteAsync(Guid photoId, CancellationToken cancellationToken);

    /// <summary>
    /// Obtém uma foto pelo seu hash (para deduplicação)
    /// </summary>
    Task<UserPhoto?> GetByFileHashAsync(string fileHash, CancellationToken cancellationToken);

    /// <summary>
    /// Deativa a foto atual do usuário (soft delete)
    /// </summary>
    Task DeactivateCurrentPhotoAsync(Guid userId, string deactivationReason, CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
