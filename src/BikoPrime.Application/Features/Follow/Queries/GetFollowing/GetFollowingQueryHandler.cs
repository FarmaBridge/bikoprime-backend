namespace BikoPrime.Application.Features.Follow.Queries.GetFollowing;

using MediatR;
using BikoPrime.Application.DTOs.User;
using BikoPrime.Application.Interfaces;
using BikoPrime.Application.DTOs.Auth;

public class GetFollowingQueryHandler : IRequestHandler<GetFollowingQuery, List<UserProfileDto>>
{
    private readonly IUserFollowRepository _userFollowRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUserPhotoRepository _userPhotoRepository;

    public GetFollowingQueryHandler(
        IUserFollowRepository userFollowRepository,
        IUserRepository userRepository,
        IUserPhotoRepository userPhotoRepository)
    {
        _userFollowRepository = userFollowRepository;
        _userRepository = userRepository;
        _userPhotoRepository = userPhotoRepository;
    }

    public async Task<List<UserProfileDto>> Handle(GetFollowingQuery request, CancellationToken cancellationToken)
    {
        var following = await _userFollowRepository.GetFollowingAsync(request.UserId);
        var followingUsers = await _userRepository.GetByIdsAsync(following.Select(f => f.FollowingId).ToList());

        var result = new List<UserProfileDto>();
        foreach (var user in followingUsers)
        {
            var isFollowing = false;
            if (request.CurrentUserId.HasValue && request.CurrentUserId.Value != user.Id)
            {
                isFollowing = await _userFollowRepository.IsFollowingAsync(request.CurrentUserId.Value, user.Id);
            }
            var latestPhoto = await _userPhotoRepository.GetLatestByUserIdAsync(user.Id, cancellationToken);
            result.Add(MapToDto(user, isFollowing, latestPhoto?.Id));
        }

        return result;
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
