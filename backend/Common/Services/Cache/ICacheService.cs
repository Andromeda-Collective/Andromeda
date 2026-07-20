namespace Andromeda.Common.Services.Cache;

public interface ICacheService
{
    Task<T?> GetAsync<T>(string key, CancellationToken ct = default);

    Task SetAsync<T>(
        string key,
        T value,
        CacheEntryOptions options,
        CancellationToken ct = default);

    Task RemoveAsync(string key, CancellationToken ct = default);

    Task<T> GetOrCreateAsync<T>(
    string key,
    Func<CancellationToken, Task<T>> factory,
    CacheEntryOptions options,
    CancellationToken ct = default);
}