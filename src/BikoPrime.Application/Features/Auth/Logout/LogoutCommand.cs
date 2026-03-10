namespace BikoPrime.Application.Features.Auth.Logout;

using MediatR;

public class LogoutCommand : IRequest<Unit>
{
    public Guid UserId { get; set; }
    
    public string? RefreshToken { get; set; }
}
