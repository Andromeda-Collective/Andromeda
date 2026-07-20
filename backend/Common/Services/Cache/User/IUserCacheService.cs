
namespace Andromeda.Common.Services.Cache.User;

public interface IUserCacheService
{
    Task<CachedUserSnapshot?> GetAsync(Guid userId, CancellationToken ct = default);
    Task SetAsync(Guid userId, CachedUserSnapshot snapshot, CancellationToken ct = default);
    Task<CachedUserSnapshot> GetOrCreateAsync(
        Guid userId,
        Func<CancellationToken, Task<CachedUserSnapshot>> factory,
        CancellationToken ct = default
    );
    Task InvalidateAsync(Guid userId, CancellationToken ct = default);
}