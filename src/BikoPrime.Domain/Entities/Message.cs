namespace BikoPrime.Domain.Entities;

public class Message
{
    public Guid Id { get; set; }

    public Guid ConversationId { get; set; }

    public Guid SenderId { get; set; }

    public string Content { get; set; } = string.Empty;

    public string Type { get; set; } = "text"; // text, image, service_share

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public Conversation? Conversation { get; set; }

    public User? Sender { get; set; }
}
