using Andromeda.Enums;

namespace Andromeda.Common.Services.Cache;

public sealed record CachedUserSnapshot(UserState State, string Role);