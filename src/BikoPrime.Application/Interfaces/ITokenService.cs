namespace BikoPrime.Application.Interfaces;

public interface ITokenService
{
    string GenerateAccessToken(Guid userId, string email);

    string GenerateRefreshToken(Guid? userId = null, string? email = null);

    (Guid? UserId, string? Email) ValidateAccessToken(string token);

    (Guid? UserId, string? Email) ValidateRefreshToken(string token);
}

