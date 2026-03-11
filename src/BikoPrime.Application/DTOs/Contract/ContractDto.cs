namespace BikoPrime.Application.DTOs.Contract;

public class ContractDto
{
    public Guid Id { get; set; }

    public Guid ServiceId { get; set; }

    public Guid ProviderId { get; set; }

    public Guid ClientId { get; set; }

    public string Status { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime? FinishedAt { get; set; }
}
