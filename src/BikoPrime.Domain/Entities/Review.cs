namespace BikoPrime.Domain.Entities;

public class Review
{
    public Guid Id { get; set; }

    public Guid ContractId { get; set; }

    public Guid AuthorId { get; set; }

    public Guid TargetUserId { get; set; }

    public int Rating { get; set; }

    public string Comment { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public Contract? Contract { get; set; }

    public User? Author { get; set; }

    public User? TargetUser { get; set; }
}
