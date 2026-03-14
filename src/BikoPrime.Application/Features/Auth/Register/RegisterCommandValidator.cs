namespace BikoPrime.Application.Features.Auth.Register;

using FluentValidation;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("O campo 'first name' é obrigatório.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("O campo 'last name' é obrigatório.");

        RuleFor(x => x.DisplayName)
            .NotEmpty().WithMessage("O campo 'display name' é obrigatório.");

        RuleFor(x => x.Gender)
            .NotEmpty().WithMessage("O campo 'gênero' é obrigatório.");

        RuleFor(x => x.Pronoun)
            .NotEmpty().WithMessage("O campo 'pronome' é obrigatório.");

        RuleFor(x => x.DateOfBirth)
            .NotEmpty().WithMessage("A data de nascimento é obrigatória.");

        RuleFor(x => x.CEP)
            .NotEmpty().WithMessage("O CEP é obrigatório.");

        RuleFor(x => x.Street)
            .NotEmpty().WithMessage("A rua/avenida é obrigatória.");

        RuleFor(x => x.StreetNumber)
            .NotEmpty().WithMessage("O número é obrigatório.");

        RuleFor(x => x.Neighborhood)
            .NotEmpty().WithMessage("O bairro é obrigatório.");

        RuleFor(x => x.City)
            .NotEmpty().WithMessage("A cidade é obrigatória.");

        RuleFor(x => x.State)
            .NotEmpty().WithMessage("O estado é obrigatório.")
            .Length(2).WithMessage("O estado deve ter 2 caracteres (UF).");

        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("O campo 'username' é obrigatório.")
            .Matches(@"^[a-zA-Z0-9._]+$").WithMessage("Username deve conter apenas letras, números, . e _")
            .Length(3, 20).WithMessage("Username deve ter entre 3 e 20 caracteres.");

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
