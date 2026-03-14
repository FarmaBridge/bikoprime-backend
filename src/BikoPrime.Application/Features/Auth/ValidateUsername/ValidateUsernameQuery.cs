namespace BikoPrime.Application.Features.Auth.ValidateUsername
{
    using MediatR;

    public class ValidateUsernameQuery : IRequest<ValidateUsernameResponse>
    {
        public string Username { get; set; } = string.Empty;
    }

    public class ValidateUsernameResponse
    {
        public bool IsAvailable { get; set; }
        public string? Message { get; set; }
    }
}
