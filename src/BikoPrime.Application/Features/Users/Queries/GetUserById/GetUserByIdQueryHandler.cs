namespace BikoPrime.Application.Features.Users.Queries.GetUserById;

using MediatR;
using BikoPrime.Application.DTOs.User;
using BikoPrime.Application.Interfaces;
using BikoPrime.Application.DTOs.Auth;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserProfileDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserFollowRepository _userFollowRepository;
    private readonly IUserPhotoRepository _userPhotoRepository;

    public GetUserByIdQueryHandler(
        IUserRepository userRepository,
        IUserFollowRepository userFollowRepository,
        IUserPhotoRepository userPhotoRepository)
    {
        _userRepository = userRepository;
        _userFollowRepository = userFollowRepository;
        _userPhotoRepository = userPhotoRepository;
    }

    public async Task<UserProfileDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(Guid.Parse(request.UserId));
        if (user == null)
            throw new KeyNotFoundException($"User with id {request.UserId} not found");

        var isFollowing = false;
        if (!string.IsNullOrEmpty(request.CurrentUserId) && Guid.Parse(request.CurrentUserId) != Guid.Parse(request.UserId))
        {
            isFollowing = await _userFollowRepository.IsFollowingAsync(Guid.Parse(request.CurrentUserId), Guid.Parse(request.UserId));
        }

        var latestPhoto = await _userPhotoRepository.GetLatestByUserIdAsync(user.Id, cancellationToken);
        return MapToDto(user, isFollowing, latestPhoto?.Id);
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
