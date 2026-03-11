namespace BikoPrime.Application.DTOs.Message;

public class ConversationDto
{
    public Guid Id { get; set; }

    public List<Guid> ParticipantIds { get; set; } = new List<Guid>();

    public MessageDto? LastMessage { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
