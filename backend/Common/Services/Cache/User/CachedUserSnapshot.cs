using Andromeda.Enums;

namespace Andromeda.Common.Services.Cache.User;

public sealed record CachedUserSnapshot(UserState State, string Role);