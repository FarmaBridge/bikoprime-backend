namespace BikoPrime.Application.Features.Auth.ValidateEmail
{
    using MediatR;

    public class ValidateEmailQuery : IRequest<ValidateEmailResponse>
    {
        public string Email { get; set; } = string.Empty;
    }

    public class ValidateEmailResponse
    {
        public bool IsAvailable { get; set; }
        public string? Message { get; set; }
    }
}
