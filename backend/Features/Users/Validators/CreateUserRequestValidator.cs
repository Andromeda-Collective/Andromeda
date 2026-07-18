using Andromeda.Features.Users.DTOs;
using FluentValidation;

namespace Andromeda.Features.Users.Validators;

public sealed class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserRequestValidator()
    {
        RuleFor(r => r.FirstName)
            .NotEmpty()
            .MaximumLength(255);
        
        RuleFor(r => r.LastName)
            .NotEmpty()
            .MaximumLength(255);

        RuleFor(r => r.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(255);

        RuleFor(request => request.Username)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(50)
            .Matches("^[a-zA-Z0-9_]+$").WithMessage("Username can only contain letters, numbers, and underscores");

        RuleFor(r => r.Password)
            .NotEmpty()
            .MinimumLength(8);

        RuleFor(r => r.ConfirmPassword)
            .Equal(r => r.Password);

        RuleFor(r => r.Role)
            .IsInEnum()
            .NotEqual(Enums.Roles.Owner).WithMessage("امکان ایجاد کاربر با رول Owner وجود ندارد");
    }
}