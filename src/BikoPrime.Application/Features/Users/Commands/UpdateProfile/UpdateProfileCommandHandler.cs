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
    private readonly IUserPhotoRepository _userPhotoRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UpdateProfileCommandHandler(
        IUserRepository userRepository,
        IUserPhotoRepository userPhotoRepository,
        IHttpContextAccessor httpContextAccessor)
    {
        _userRepository = userRepository;
        _userPhotoRepository = userPhotoRepository;
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
        var latestPhoto = await _userPhotoRepository.GetLatestByUserIdAsync(user.Id, cancellationToken);

        return MapToDto(user, false, latestPhoto?.Id);
    }

    private static UserProfileDto MapToDto(BikoPrime.Domain.Entities.User user, bool isFollowing, Guid? photoId)
    {
        return new UserProfileDto
        {
            Id = user.Id,
            Name = user.DisplayName ?? user.UserName ?? string.Empty,
            DisplayName = user.DisplayName ?? user.UserName ?? string.Empty,
            UserName = user.UserName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            PhoneNumber = user.PhoneNumber ?? string.Empty,
            PhotoId = photoId,
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
