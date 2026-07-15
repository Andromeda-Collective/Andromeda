using Andromeda.Features.Auth.DTOs;
using FluentValidation;

namespace Andromeda.Features.Auth.Validator;

public sealed class LogoutRequestValidator : AbstractValidator<LogoutRequest>
{
    public LogoutRequestValidator()
    {
        RuleFor(r => r.RefreshToken)
            .NotEmpty();
    }
}