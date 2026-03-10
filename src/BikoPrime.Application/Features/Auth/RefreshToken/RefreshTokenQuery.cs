namespace BikoPrime.Application.Features.Auth.RefreshToken;

using MediatR;
using BikoPrime.Application.DTOs.Auth;

public class RefreshTokenQuery : IRequest<RefreshTokenResponseDto>
{
    public string RefreshToken { get; set; } = string.Empty;
}
