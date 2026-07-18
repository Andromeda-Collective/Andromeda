namespace Andromeda.Features.Users.DTOs;


public sealed record UpdateUserByOwnerRequest(string FirstName,
    string LastName,
    string Username,
    string Email
);
