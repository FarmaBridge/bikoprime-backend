namespace BikoPrime.Application.DTOs.Message;

public class SendMessageDto
{
    public string Content { get; set; } = string.Empty;

    public string Type { get; set; } = "text";
}
