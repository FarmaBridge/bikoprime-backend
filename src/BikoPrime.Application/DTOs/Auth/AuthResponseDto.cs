namespace BikoPrime.Application.DTOs.Auth;

public class AuthResponseDto
{
    public UserDto User { get; set; } = new();
    
    public string Token { get; set; } = string.Empty;
    
    public string RefreshToken { get; set; } = string.Empty;
}
