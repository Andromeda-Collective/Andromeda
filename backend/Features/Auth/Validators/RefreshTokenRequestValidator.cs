using Andromeda.Features.Auth.DTOs;
using FluentValidation;

namespace Andromeda.Features.Auth.Validator;

public sealed class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequest>
{
    public RefreshTokenRequestValidator()
    {
        RuleFor(r => r.UserId)
            .NotEmpty();

        RuleFor(r => r.RefreshToken)
            .NotEmpty();
    }
}