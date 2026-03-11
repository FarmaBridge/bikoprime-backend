namespace BikoPrime.Application.Features.Users.Queries.GetRecommendedUsers;

using MediatR;
using BikoPrime.Application.DTOs.User;
using BikoPrime.Application.Interfaces;
using BikoPrime.Application.DTOs.Auth;

public class GetRecommendedUsersQueryHandler : IRequestHandler<GetRecommendedUsersQuery, List<UserProfileDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserFollowRepository _userFollowRepository;

    public GetRecommendedUsersQueryHandler(IUserRepository userRepository, IUserFollowRepository userFollowRepository)
    {
        _userRepository = userRepository;
        _userFollowRepository = userFollowRepository;
    }

    public async Task<List<UserProfileDto>> Handle(GetRecommendedUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _userRepository.GetRecommendedAsync();
        
        var result = new List<UserProfileDto>();
        foreach (var user in users)
        {
            var isFollowing = false;
            if (!string.IsNullOrEmpty(request.CurrentUserId) && Guid.Parse(request.CurrentUserId) != user.Id)
            {
                isFollowing = await _userFollowRepository.IsFollowingAsync(Guid.Parse(request.CurrentUserId), user.Id);
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
