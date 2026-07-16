namespace Andromeda.Features.Auth.DTOs;

public sealed record RegisterUserRequest(
    string FirstName,
    string LastName,
    string Email,
    string Username,
    string Password,
    string ConfirmPassword
);