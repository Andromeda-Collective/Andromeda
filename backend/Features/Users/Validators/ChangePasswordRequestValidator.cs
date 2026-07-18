using Andromeda.Features.Users.DTOs;
using FluentValidation;

namespace Andromeda.Features.Users.Validators;


public sealed class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
{
    public ChangePasswordRequestValidator()
    {
        RuleFor(r => r.CurrentPassword)
            .NotEmpty();
            
        RuleFor(r => r.NewPassword)
            .NotEmpty()
            .MinimumLength(8);

        RuleFor(r => r.ConfirmNewPassword)
            .Equal(r => r.NewPassword).WithMessage("رمز عبور جدید و تکرار آن یکسان نیستند");
    }
}