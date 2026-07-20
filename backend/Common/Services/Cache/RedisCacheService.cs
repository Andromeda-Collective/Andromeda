using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace Andromeda.Common.Services.Cache;

public sealed class RedisCacheService : ICacheService
{
    private readonly IDistributedCache _cache;
    private static readonly JsonSerializerOptions SerializerOptions =
    new(JsonSerializerDefaults.Web);

    public RedisCacheService(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<T?> GetAsync<T>(
        string key,
        CancellationToken ct = default)
    {
        var json = await _cache.GetStringAsync(key, ct);

        return json is null
            ? default
            : JsonSerializer.Deserialize<T>(json, SerializerOptions);
    }

    public async Task SetAsync<T>(
        string key,
        T value,
        CacheEntryOptions options,
        CancellationToken ct = default)
    {
        var distributedOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = options.AbsoluteExpirationRelativeToNow,
            SlidingExpiration = options.SlidingExpiration
        };
        
        var json = JsonSerializer.Serialize(value, SerializerOptions);

        await _cache.SetStringAsync(key, json, distributedOptions, ct);
    }

    public Task RemoveAsync(
        string key,
        CancellationToken ct = default)
    {
        return _cache.RemoveAsync(key, ct);
    }


    public async Task<T> GetOrCreateAsync<T>(
    string key,
    Func<CancellationToken, Task<T>> factory,
    CacheEntryOptions options,
    CancellationToken ct = default)
    {
        var cached = await GetAsync<T>(key, ct);

        if (cached is not null)
            return cached;

        var value = await factory(ct);

        await SetAsync(key, value, options, ct);

        return value;
    }
}