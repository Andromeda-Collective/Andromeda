namespace Andromeda.Features.Users.DTOs;

public sealed record UpdateOwnProfileRequest(
    string FirstName,
    string LastName,
    string Username
);
