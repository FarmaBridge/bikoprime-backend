namespace BikoPrime.Application.Features.Auth.GoogleAuth;

using MediatR;
using Microsoft.AspNetCore.Identity;
using BikoPrime.Application.DTOs.Auth;
using BikoPrime.Application.Interfaces;
using BikoPrime.Domain.Entities;
using BikoPrime.Domain.Exceptions;

public class GoogleAuthCommandHandler : IRequestHandler<GoogleAuthCommand, AuthResponseDto>
{
    private readonly IGoogleTokenValidator _googleTokenValidator;
    private readonly UserManager<User> _userManager;
    private readonly ITokenService _tokenService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public GoogleAuthCommandHandler(
        IGoogleTokenValidator googleTokenValidator,
        UserManager<User> userManager,
        ITokenService tokenService,
        IRefreshTokenRepository refreshTokenRepository)
    {
        _googleTokenValidator = googleTokenValidator;
        _userManager = userManager;
        _tokenService = tokenService;
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task<AuthResponseDto> Handle(GoogleAuthCommand request, CancellationToken cancellationToken)
    {
        var (email, name, picture) = await _googleTokenValidator.ValidateAndExtractAsync(request.IdToken);

        var user = await _userManager.FindByEmailAsync(email);

        if (user == null)
        {
            user = new User
            {
                Id = Guid.NewGuid(),
                UserName = GenerateUsernameFromEmail(email),
                Email = email,
                AvatarUrl = picture ?? string.Empty,
                PhoneNumber = string.Empty,
                Latitude = 0,
                Longitude = 0,
                Address = string.Empty,
                Bio = string.Empty,
                Rating = 0,
                FollowersCount = 0,
                FollowingCount = 0,
                Services = new List<string>(),
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new DomainException(errors, "VALIDATION_ERROR");
            }
        }

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

    private static string GenerateUsernameFromEmail(string email)
    {
        var username = email.Split('@')[0].Replace(".", "_");
        return username.Length > 20 ? username[..20] : username;
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
