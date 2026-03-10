namespace BikoPrime.Application.Features.Auth.RefreshToken;

using MediatR;
using BikoPrime.Application.DTOs.Auth;
using BikoPrime.Application.Interfaces;
using BikoPrime.Domain.Exceptions;

public class RefreshTokenQueryHandler : IRequestHandler<RefreshTokenQuery, RefreshTokenResponseDto>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly ITokenService _tokenService;

    public RefreshTokenQueryHandler(
        IRefreshTokenRepository refreshTokenRepository,
        ITokenService tokenService)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _tokenService = tokenService;
    }

    public async Task<RefreshTokenResponseDto> Handle(RefreshTokenQuery request, CancellationToken cancellationToken)
    {
        var refreshToken = await _refreshTokenRepository.GetValidTokenAsync(request.RefreshToken);

        if (refreshToken == null || refreshToken.IsRevoked || refreshToken.ExpiresAt < DateTime.UtcNow)
            throw new DomainException("Token inválido ou expirado", "INVALID_REFRESH_TOKEN");

        var (userId, email) = _tokenService.ValidateRefreshToken(request.RefreshToken);

        if (userId == null || string.IsNullOrWhiteSpace(email))
            throw new DomainException("Token inválido ou expirado", "INVALID_REFRESH_TOKEN");

        var newAccessToken = _tokenService.GenerateAccessToken(userId.Value, email);
        var newRefreshToken = _tokenService.GenerateRefreshToken(userId.Value, email);

        await _refreshTokenRepository.RevokeAsync(request.RefreshToken);

        var refreshTokenEntity = new Domain.Entities.RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId.Value,
            Token = newRefreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow,
            IsRevoked = false
        };

        await _refreshTokenRepository.CreateAsync(refreshTokenEntity);

        return new RefreshTokenResponseDto
        {
            Token = newAccessToken,
            RefreshToken = newRefreshToken
        };
    }
}
