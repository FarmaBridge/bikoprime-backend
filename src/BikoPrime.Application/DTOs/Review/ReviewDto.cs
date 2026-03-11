namespace BikoPrime.Application.DTOs.Review;

public class ReviewDto
{
    public Guid Id { get; set; }

    public Guid ContractId { get; set; }

    public Guid AuthorId { get; set; }

    public Guid TargetUserId { get; set; }

    public int Rating { get; set; }

    public string Comment { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}
