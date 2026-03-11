namespace BikoPrime.Application.Features.Follow.Queries.GetFollowing;

using MediatR;
using BikoPrime.Application.DTOs.User;
using BikoPrime.Application.Interfaces;
using BikoPrime.Application.DTOs.Auth;

public class GetFollowingQueryHandler : IRequestHandler<GetFollowingQuery, List<UserProfileDto>>
{
    private readonly IUserFollowRepository _userFollowRepository;
    private readonly IUserRepository _userRepository;

    public GetFollowingQueryHandler(IUserFollowRepository userFollowRepository, IUserRepository userRepository)
    {
        _userFollowRepository = userFollowRepository;
        _userRepository = userRepository;
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
            result.Add(MapToDto(user, isFollowing));
        }

        return result;
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
