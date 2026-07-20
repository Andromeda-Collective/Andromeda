
namespace Andromeda.Common.Services.Cache.User;

public sealed class UserCacheService : IUserCacheService
{
    private readonly ICacheService _cache;

    public UserCacheService(ICacheService cache)
    {
        _cache = cache;
    }

    public async Task<CachedUserSnapshot?> GetAsync(Guid userId, CancellationToken ct = default)
    {
        return await _cache.GetAsync<CachedUserSnapshot>(UserCacheKeys.Snapshot(userId), ct);
    }

    public async Task SetAsync(Guid userId, CachedUserSnapshot snapshot, CancellationToken ct = default)
    {
        await _cache.SetAsync(
            UserCacheKeys.Snapshot(userId),
            snapshot, new CacheEntryOptions { AbsoluteExpirationRelativeToNow = UserCacheOptions.SnapshotTtl },
            ct
        );
    }

    public async Task InvalidateAsync(Guid userId, CancellationToken ct = default)
    {
        await _cache.RemoveAsync(UserCacheKeys.Snapshot(userId), ct);
    }

    public Task<CachedUserSnapshot> GetOrCreateAsync(
    Guid userId,
    Func<CancellationToken, Task<CachedUserSnapshot>> factory,
    CancellationToken ct = default)
    {
        return _cache.GetOrCreateAsync(
            UserCacheKeys.Snapshot(userId),
            factory,
            new CacheEntryOptions { AbsoluteExpirationRelativeToNow = UserCacheOptions.SnapshotTtl },
            ct);
    }
}