namespace BikoPrime.Domain.Entities;

public class Conversation
{
    public Guid Id { get; set; }

    public List<Guid> ParticipantIds { get; set; } = new List<Guid>();

    public Guid? LastMessageId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public Message? LastMessage { get; set; }

    public List<Message> Messages { get; set; } = new List<Message>();
}
