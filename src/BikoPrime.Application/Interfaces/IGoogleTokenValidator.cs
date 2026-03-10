namespace BikoPrime.Application.Interfaces;

public interface IGoogleTokenValidator
{
    Task<(string Email, string Name, string? Picture)> ValidateAndExtractAsync(string idToken);
}
