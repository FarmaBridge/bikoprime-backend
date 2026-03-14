namespace BikoPrime.Application.Features.Auth.Login;

using MediatR;
using Microsoft.AspNetCore.Identity;
using BikoPrime.Application.DTOs.Auth;
using BikoPrime.Application.Interfaces;
using BikoPrime.Domain.Entities;
using BikoPrime.Domain.Exceptions;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponseDto>
{
    private readonly UserManager<User> _userManager;
    private readonly ITokenService _tokenService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public LoginCommandHandler(
        UserManager<User> userManager,
        ITokenService tokenService,
        IRefreshTokenRepository refreshTokenRepository)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task<AuthResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null)
            throw new DomainException("E-mail ou senha incorretos", "INVALID_CREDENTIALS");

        var passwordValid = await _userManager.CheckPasswordAsync(user, request.Password);

        if (!passwordValid)
            throw new DomainException("E-mail ou senha incorretos", "INVALID_CREDENTIALS");

        var accessToken = _tokenService.GenerateAccessToken(user.Id, user.Email!);
        var refreshToken = _tokenService.GenerateRefreshToken(user.Id, user.Email);

        var existingRefreshToken = await _refreshTokenRepository.GetByUserIdAsync(user.Id);
        if (existingRefreshToken != null)
        {
            await _refreshTokenRepository.RevokeAsync(existingRefreshToken.Token);
        }

        var refreshTokenEntity = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow,
            IsRevoked = false
        };

        await _refreshTokenRepository.CreateAsync(refreshTokenEntity);

        return new AuthResponseDto
        {
            User = MapToUserDto(user),
            Token = accessToken,
            RefreshToken = refreshToken
        };
    }

    private static UserDto MapToUserDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            FirstName = user.FirstName ?? string.Empty,
            LastName = user.LastName ?? string.Empty,
            DisplayName = user.DisplayName ?? string.Empty,
            UserName = user.UserName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            PhoneNumber = user.PhoneNumber ?? string.Empty,
            Gender = user.Gender,
            Pronoun = user.Pronoun,
            DateOfBirth = user.DateOfBirth,
            CEP = user.CEP,
            AvatarUrl = user.AvatarUrl,
            Location = new LocationDto
            {
                Latitude = user.Latitude,
                Longitude = user.Longitude,
                Address = user.Address
            },
            Rating = user.Rating,
            FollowersCount = user.FollowersCount,
            FollowingCount = user.FollowingCount,
            Bio = user.Bio,
            Services = user.Services ?? new List<string>()
        };
    }
}
