namespace Andromeda.Common.Services.Cache.User;

public static class UserCacheKeys
{
    public static string Snapshot(Guid userId)
        => $"user:snapshot:{userId:N}";
}