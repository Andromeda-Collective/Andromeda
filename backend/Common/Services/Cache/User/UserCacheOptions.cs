namespace Andromeda.Common.Services.Cache.User;

public static class UserCacheOptions
{
    public static readonly TimeSpan SnapshotTtl =
        TimeSpan.FromMinutes(5);
}