namespace BikoPrime.Domain.Entities;

public class UserFollow
{
    public Guid Id { get; set; }

    public Guid FollowerId { get; set; }

    public Guid FollowingId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public User? Follower { get; set; }

    public User? Following { get; set; }
}
