namespace BikoPrime.Application.Features.Auth.Register;

using MediatR;
using Microsoft.AspNetCore.Identity;
using BikoPrime.Application.DTOs.Auth;
using BikoPrime.Application.Interfaces;
using BikoPrime.Domain.Entities;
using BikoPrime.Domain.Exceptions;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, RegisterResponseDto>
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

    public async Task<RegisterResponseDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        if (await _userManager.FindByEmailAsync(request.Email) != null)
            throw new DomainException("E-mail já cadastrado", "EMAIL_IN_USE");

        if (await _userManager.FindByNameAsync(request.UserName) != null)
            throw new DomainException("Username já em uso", "USERNAME_IN_USE");

        var user = new User
        {
            Id = Guid.NewGuid(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            DisplayName = request.DisplayName,
            UserName = request.UserName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            Gender = request.Gender,
            Pronoun = request.Pronoun,
            DateOfBirth = request.DateOfBirth.HasValue 
                ? DateTime.SpecifyKind(request.DateOfBirth.Value, DateTimeKind.Utc) 
                : (DateTime?)null,
            CEP = request.CEP,
            Street = request.Street,
            StreetNumber = request.StreetNumber,
            Complement = request.Complement,
            Neighborhood = request.Neighborhood,
            City = request.City,
            State = request.State,
            Latitude = request.Location?.Latitude ?? 0,
            Longitude = request.Location?.Longitude ?? 0,
            Address = BuildFullAddress(request),
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

        return new RegisterResponseDto
        {
            User = MapToUserDto(user),
            Message = "Conta criada com sucesso."
        };
    }

    private static string BuildFullAddress(RegisterCommand request)
    {
        var parts = new List<string>();
        
        if (!string.IsNullOrWhiteSpace(request.Street))
            parts.Add(request.Street);
        
        if (!string.IsNullOrWhiteSpace(request.StreetNumber))
            parts.Add(request.StreetNumber);
        
        if (!string.IsNullOrWhiteSpace(request.Complement))
            parts.Add(request.Complement);
        
        if (!string.IsNullOrWhiteSpace(request.Neighborhood))
            parts.Add(request.Neighborhood);
        
        if (!string.IsNullOrWhiteSpace(request.City))
            parts.Add(request.City);
        
        if (!string.IsNullOrWhiteSpace(request.State))
            parts.Add(request.State);
        
        if (!string.IsNullOrWhiteSpace(request.CEP))
            parts.Add(request.CEP);

        return string.Join(", ", parts.Where(p => !string.IsNullOrWhiteSpace(p)));
    }

    private static UserDto MapToUserDto(User user, Guid? photoId = null)
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
            Street = user.Street,
            StreetNumber = user.StreetNumber,
            Complement = user.Complement,
            Neighborhood = user.Neighborhood,
            City = user.City,
            State = user.State,
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
            Services = user.Services ?? new List<string>()
        };
    }
}

