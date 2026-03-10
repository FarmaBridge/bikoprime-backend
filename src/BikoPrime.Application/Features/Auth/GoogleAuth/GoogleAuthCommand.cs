namespace BikoPrime.Application.Features.Auth.GoogleAuth;

using MediatR;
using BikoPrime.Application.DTOs.Auth;

public class GoogleAuthCommand : IRequest<AuthResponseDto>
{
    public string IdToken { get; set; } = string.Empty;
}
