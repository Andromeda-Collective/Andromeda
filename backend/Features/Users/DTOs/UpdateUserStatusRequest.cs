using Andromeda.Enums;

namespace Andromeda.Features.Users.DTOs;

public sealed record UpdateUserStatusRequest(UserState State);