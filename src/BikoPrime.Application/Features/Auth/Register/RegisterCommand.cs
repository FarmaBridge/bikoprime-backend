namespace BikoPrime.Application.Features.Auth.Register;

using MediatR;
using BikoPrime.Application.DTOs.Auth;

public class RegisterCommand : IRequest<RegisterResponseDto>
{
    public string Name { get; set; } = string.Empty;
    
    public string UserName { get; set; } = string.Empty;
    
    public string Email { get; set; } = string.Empty;
    
    public string Phone { get; set; } = string.Empty;
    
    public string Password { get; set; } = string.Empty;
    
    public string? AvatarUrl { get; set; }
    
    public LocationDto? Location { get; set; }
    
    public string? Bio { get; set; }
}
