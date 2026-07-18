using Andromeda.Enums;

namespace Andromeda.Features.Users.DTOs;

public sealed record UserProfileResponse(
    Guid Id,
    string Username,
    string Email,
    string FirstName,
    string LastName,
    string Role,
    UserState State,
    string ProfileImageUrl,
    int ActiveSessionCount
);