namespace BikoPrime.Application.Features.Auth.Register;

using FluentValidation;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("O campo 'name' é obrigatório.");

        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("O campo 'username' é obrigatório.")
            .Matches(@"^[a-zA-Z0-9._]+$").WithMessage("Username deve conter apenas letras, números, . e _");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("O campo 'email' é obrigatório.")
            .EmailAddress().WithMessage("O e-mail deve ser válido.");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("O campo 'phone' é obrigatório.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("O campo 'password' é obrigatório.")
            .MinimumLength(6).WithMessage("Senha deve ter no mínimo 6 caracteres.");
    }
}
