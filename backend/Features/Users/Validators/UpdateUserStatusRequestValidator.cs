using Andromeda.Features.Users.DTOs;
using FluentValidation;

namespace Andromeda.Features.Users.Validators;

public sealed class UpdateUserStatusRequestValidator : AbstractValidator<UpdateUserStatusRequest>
{
    public UpdateUserStatusRequestValidator()
    {
        RuleFor(r => r.State)
            .IsInEnum();
    }
}