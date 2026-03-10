namespace BikoPrime.Application.Features.Auth.RefreshToken;

using FluentValidation;

public class RefreshTokenQueryValidator : AbstractValidator<RefreshTokenQuery>
{
    public RefreshTokenQueryValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("O campo 'refreshToken' é obrigatório.");
    }
}
