namespace BikoPrime.Application.Features.Auth.ValidateEmail
{
    using MediatR;
    using Microsoft.AspNetCore.Identity;
    using BikoPrime.Domain.Entities;

    public class ValidateEmailQueryHandler : IRequestHandler<ValidateEmailQuery, ValidateEmailResponse>
    {
        private readonly UserManager<User> _userManager;

        public ValidateEmailQueryHandler(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ValidateEmailResponse> Handle(ValidateEmailQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Email))
            {
                return new ValidateEmailResponse
                {
                    IsAvailable = false,
                    Message = "E-mail é obrigatório."
                };
            }

            var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!System.Text.RegularExpressions.Regex.IsMatch(request.Email, emailPattern))
            {
                return new ValidateEmailResponse
                {
                    IsAvailable = false,
                    Message = "E-mail inválido."
                };
            }

            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            
            if (existingUser != null)
            {
                return new ValidateEmailResponse
                {
                    IsAvailable = false,
                    Message = "Este e-mail já está cadastrado."
                };
            }

            return new ValidateEmailResponse
            {
                IsAvailable = true,
                Message = "E-mail disponível."
            };
        }
    }
}
