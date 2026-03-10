namespace BikoPrime.Infrastructure.Services;

using Google.Apis.Auth;
using BikoPrime.Application.Interfaces;
using BikoPrime.Domain.Exceptions;

public class GoogleTokenValidator : IGoogleTokenValidator
{
    private readonly string _clientId;

    public GoogleTokenValidator(string clientId)
    {
        _clientId = clientId;
    }

    public async Task<(string Email, string Name, string? Picture)> ValidateAndExtractAsync(string idToken)
    {
        try
        {
            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken);

            if (payload == null || string.IsNullOrWhiteSpace(payload.Email))
                throw new DomainException("Token inválido ou expirado", "INVALID_GOOGLE_TOKEN");

            var email = payload.Email;
            var name = payload.Name ?? string.Empty;
            var picture = payload.Picture;

            return (email, name, picture);
        }
        catch (InvalidOperationException)
        {
            throw new DomainException("Token inválido ou expirado", "INVALID_GOOGLE_TOKEN");
        }
    }
}
