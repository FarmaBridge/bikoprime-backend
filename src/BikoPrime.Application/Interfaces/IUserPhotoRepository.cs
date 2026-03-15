namespace BikoPrime.Application.Interfaces;

using BikoPrime.Domain.Entities;

public interface IUserPhotoRepository
{
    Task<UserPhoto> AddAsync(UserPhoto userPhoto, CancellationToken cancellationToken);

    Task<UserPhoto?> GetByIdAsync(Guid photoId, CancellationToken cancellationToken);

    Task<UserPhoto?> GetLatestByUserIdAsync(Guid userId, CancellationToken cancellationToken);

    Task<List<UserPhoto>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);

    Task DeleteAsync(Guid photoId, CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
