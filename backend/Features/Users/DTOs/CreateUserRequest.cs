using Andromeda.Enums;

namespace Andromeda.Features.Users.DTOs;

public sealed record CreateUserRequest(
    string FirstName,
    string LastName,
    string Email,
    string Username,
    string Password,
    string ConfirmPassword,
    Roles Role
);