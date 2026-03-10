namespace BikoPrime.Application.Features.Auth.Login;

using MediatR;
using BikoPrime.Application.DTOs.Auth;

public class LoginCommand : IRequest<AuthResponseDto>
{
    public string Email { get; set; } = string.Empty;
    
    public string Password { get; set; } = string.Empty;
}
