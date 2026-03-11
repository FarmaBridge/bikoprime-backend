namespace BikoPrime.Application.Interfaces;

using BikoPrime.Domain.Entities;

public interface IReviewRepository
{
    Task<Review?> GetByIdAsync(Guid id);
    
    Task<List<Review>> GetByTargetUserAsync(Guid targetUserId);
    
        Task<List<Review>> GetByTargetUserIdAsync(Guid targetUserId);
        
        Task<List<Review>> GetByAuthorAsync(Guid authorId);
        
        Task<List<Review>> GetByAuthorIdAsync(Guid authorId);
        
        Task<List<Review>> GetByContractAsync(Guid contractId);
        
        Task<Review?> GetByContractAndTargetUserAsync(Guid contractId, Guid targetUserId);
    
    Task CreateAsync(Review review);
    
    Task UpdateAsync(Review review);
}
