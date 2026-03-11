namespace BikoPrime.Application.DTOs.Auth;

public class RegisterResponseDto
{
    public UserDto User { get; set; } = new();
    
    public string Message { get; set; } = "Conta criada com sucesso.";
}
