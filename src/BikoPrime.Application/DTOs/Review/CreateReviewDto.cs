namespace BikoPrime.Application.DTOs.Review;

public class CreateReviewDto
{
    public Guid ContractId { get; set; }

    public Guid TargetUserId { get; set; }

    public int Rating { get; set; }

    public string Comment { get; set; } = string.Empty;
}
