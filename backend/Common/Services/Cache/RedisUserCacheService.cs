using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace Andromeda.Common.Services.Cache;

public sealed class RedisUserCacheService : IUserCacheService
{
    private static readonly TimeSpan Ttl = TimeSpan.FromMinutes(5);
    private readonly IDistributedCache _cache;

    public RedisUserCacheService(IDistributedCache cache)
    {
        _cache = cache;
    }

    private static string Key(Guid userId) => $"user:snapshot:{userId:N}";

    public async Task<CachedUserSnapshot?> GetAsync(Guid userId, CancellationToken ct = default)
    {
        var json = await _cache.GetStringAsync(Key(userId), ct);
        return json is null ? null : JsonSerializer.Deserialize<CachedUserSnapshot>(json);
    }

    public async Task SetAsync(Guid userId, CachedUserSnapshot snapshot, CancellationToken ct = default)
    {
        var json = JsonSerializer.Serialize(snapshot);
        await _cache.SetStringAsync(Key(userId), json,
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = Ttl }, ct);
    }

    public async Task InvalidateAsync(Guid userId, CancellationToken ct = default)
    {
        await _cache.RemoveAsync(Key(userId), ct);
    }
}