using MediatR;
using BikoPrime.Application.DTOs.User;
using BikoPrime.Application.Interfaces;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using BikoPrime.Application.DTOs.Auth;

namespace BikoPrime.Application.Features.Users.Commands.UpdateProfile;

public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, UserProfileDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UpdateProfileCommandHandler(IUserRepository userRepository, IHttpContextAccessor httpContextAccessor)
    {
        _userRepository = userRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<UserProfileDto> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var parsedUserId))
        {
            throw new UnauthorizedAccessException("User not authenticated");
        }

        var user = await _userRepository.GetByIdAsync(parsedUserId);
        if (user == null)
            throw new KeyNotFoundException($"User with id {parsedUserId} not found");

        if (!string.IsNullOrEmpty(request.Name))
            user.UserName = request.Name;

        if (!string.IsNullOrEmpty(request.UserName))
            user.UserName = request.UserName;

        if (!string.IsNullOrEmpty(request.Email))
            user.Email = request.Email;

        if (!string.IsNullOrEmpty(request.PhoneNumber))
            user.PhoneNumber = request.PhoneNumber;

        if (!string.IsNullOrEmpty(request.AvatarUrl))
            user.AvatarUrl = request.AvatarUrl;

        if (request.Latitude.HasValue)
            user.Latitude = request.Latitude.Value;

        if (request.Longitude.HasValue)
            user.Longitude = request.Longitude.Value;

        if (request.Address != null)
            user.Address = request.Address;

        if (!string.IsNullOrEmpty(request.Bio))
            user.Bio = request.Bio;

        user.UpdatedAt = DateTime.UtcNow;

        await _userRepository.UpdateAsync(user);

        return MapToDto(user, false);
    }

    private UserProfileDto MapToDto(BikoPrime.Domain.Entities.User user, bool isFollowing)
    {
        return new UserProfileDto
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
            Services = user.Services ?? new List<string>(),
            IsFollowing = isFollowing
        };
    }
}
