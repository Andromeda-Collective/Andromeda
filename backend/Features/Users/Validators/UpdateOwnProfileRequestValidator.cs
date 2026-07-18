using Andromeda.Features.Users.DTOs;
using FluentValidation;

namespace Andromeda.Features.Users.Validators;

public sealed class UpdateOwnProfileRequestValidator : AbstractValidator<UpdateOwnProfileRequest>
{
    public UpdateOwnProfileRequestValidator()
    {
        RuleFor(r => r.FirstName)
            .NotEmpty()
            .MaximumLength(255);

        RuleFor(r => r.LastName)
            .NotEmpty()
            .MaximumLength(255);

        RuleFor(request => request.Username)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(50)
            .Matches("^[a-zA-Z0-9_]+$").WithMessage("Username can only contain letters, numbers, and underscores");

    }
}