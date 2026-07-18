using Andromeda.Enums;

namespace Andromeda.Features.Users.DTOs;

public sealed record UserListFilter(
    string? Username,
    string? Email,
    Roles? Role,
    UserState? State
);