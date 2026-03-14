namespace BikoPrime.Application.Features.Auth.ValidateUsername
{
    using MediatR;
    using Microsoft.AspNetCore.Identity;
    using BikoPrime.Domain.Entities;

    public class ValidateUsernameQueryHandler : IRequestHandler<ValidateUsernameQuery, ValidateUsernameResponse>
    {
        private readonly UserManager<User> _userManager;

        public ValidateUsernameQueryHandler(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ValidateUsernameResponse> Handle(ValidateUsernameQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Username))
            {
                return new ValidateUsernameResponse
                {
                    IsAvailable = false,
                    Message = "Username é obrigatório."
                };
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(request.Username, @"^[a-zA-Z0-9._]+$"))
            {
                return new ValidateUsernameResponse
                {
                    IsAvailable = false,
                    Message = "Username deve conter apenas letras, números, . e _"
                };
            }

            if (request.Username.Length < 3 || request.Username.Length > 20)
            {
                return new ValidateUsernameResponse
                {
                    IsAvailable = false,
                    Message = "Username deve ter entre 3 e 20 caracteres."
                };
            }

            var existingUser = await _userManager.FindByNameAsync(request.Username);
            
            if (existingUser != null)
            {
                return new ValidateUsernameResponse
                {
                    IsAvailable = false,
                    Message = "Este username já está em uso."
                };
            }

            return new ValidateUsernameResponse
            {
                IsAvailable = true,
                Message = "Username disponível."
            };
        }
    }
}
