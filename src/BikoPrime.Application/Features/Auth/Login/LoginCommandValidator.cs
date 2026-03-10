namespace BikoPrime.Application.Features.Auth.Login;

using FluentValidation;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("O campo 'email' é obrigatório.")
            .EmailAddress().WithMessage("O e-mail deve ser válido.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("O campo 'password' é obrigatório.");
    }
}
