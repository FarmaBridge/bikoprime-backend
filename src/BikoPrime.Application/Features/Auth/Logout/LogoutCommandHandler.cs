namespace BikoPrime.Application.Features.Auth.Logout;

using MediatR;
using BikoPrime.Application.Interfaces;
using BikoPrime.Domain.Exceptions;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Unit>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public LogoutCommandHandler(IRefreshTokenRepository refreshTokenRepository)
    {
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task<Unit> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
            throw new DomainException("Token de refresh não informado", "VALIDATION_ERROR");

        var refreshToken = await _refreshTokenRepository.GetValidTokenAsync(request.RefreshToken);

        if (refreshToken == null)
            throw new DomainException("Token inválido ou expirado", "INVALID_REFRESH_TOKEN");

        if (refreshToken.UserId != request.UserId)
            throw new DomainException("Token não pertence a este usuário", "INVALID_REFRESH_TOKEN");

        await _refreshTokenRepository.RevokeAsync(request.RefreshToken);

        return Unit.Value;
    }
}
