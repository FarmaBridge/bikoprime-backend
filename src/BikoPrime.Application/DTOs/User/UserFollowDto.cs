namespace BikoPrime.Application.DTOs.User;

public class UserFollowDto
{
    public Guid FollowerId { get; set; }
    
    public Guid FollowingId { get; set; }
    
    public DateTime CreatedAt { get; set; }
}
