namespace BikoPrime.Domain.Entities;

public class Contract
{
    public Guid Id { get; set; }

    public Guid ServiceId { get; set; }

    public Guid ProviderId { get; set; }

    public Guid ClientId { get; set; }

    public string Status { get; set; } = "pending"; // pending, confirmed, in_progress, finished, cancelled

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? FinishedAt { get; set; }

    // Navigation
    public Service? Service { get; set; }

    public User? Provider { get; set; }

    public User? Client { get; set; }

    public List<Review> Reviews { get; set; } = new List<Review>();
}
