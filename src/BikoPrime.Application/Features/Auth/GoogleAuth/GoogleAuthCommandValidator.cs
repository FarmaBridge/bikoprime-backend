namespace BikoPrime.Application.Features.Auth.GoogleAuth;

using FluentValidation;

public class GoogleAuthCommandValidator : AbstractValidator<GoogleAuthCommand>
{
    public GoogleAuthCommandValidator()
    {
        RuleFor(x => x.IdToken)
            .NotEmpty().WithMessage("O campo 'idToken' é obrigatório.");
    }
}
