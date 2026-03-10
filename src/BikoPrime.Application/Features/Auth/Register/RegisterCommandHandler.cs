namespace BikoPrime.Application.Features.Auth.Register;

using MediatR;
using Microsoft.AspNetCore.Identity;
using BikoPrime.Application.DTOs.Auth;
using BikoPrime.Application.Interfaces;
using BikoPrime.Domain.Entities;
using BikoPrime.Domain.Exceptions;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponseDto>
{
    private readonly UserManager<User> _userManager;
    private readonly ITokenService _tokenService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public RegisterCommandHandler(
        UserManager<User> userManager,
        ITokenService tokenService,
        IRefreshTokenRepository refreshTokenRepository)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task<AuthResponseDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        if (await _userManager.FindByEmailAsync(request.Email) != null)
            throw new DomainException("E-mail já cadastrado", "EMAIL_IN_USE");

        if (await _userManager.FindByNameAsync(request.UserName) != null)
            throw new DomainException("Username já em uso", "USERNAME_IN_USE");

        var user = new User
        {
            Id = Guid.NewGuid(),
            UserName = request.UserName,
            Email = request.Email,
            PhoneNumber = request.Phone,
            AvatarUrl = request.AvatarUrl ?? string.Empty,
            Latitude = request.Location?.Latitude ?? 0,
            Longitude = request.Location?.Longitude ?? 0,
            Address = request.Location?.Address ?? string.Empty,
            Bio = request.Bio ?? string.Empty,
            Rating = 0,
            FollowersCount = 0,
            FollowingCount = 0,
            Services = new List<string>(),
            CreatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new DomainException(errors, "VALIDATION_ERROR");
        }

        var accessToken = _tokenService.GenerateAccessToken(user.Id, user.Email!);
        var refreshToken = _tokenService.GenerateRefreshToken(user.Id, user.Email);

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
            Name = user.UserName ?? string.Empty,
            UserName = user.UserName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            PhoneNumber = user.PhoneNumber ?? string.Empty,
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
