
namespace Andromeda.Common.Services.Cache;

public interface IUserCacheService
{
    Task<CachedUserSnapshot?> GetAsync(Guid userId, CancellationToken ct = default);
    Task SetAsync(Guid userId, CachedUserSnapshot snapshot, CancellationToken ct = default);
    Task InvalidateAsync(Guid userId, CancellationToken ct = default);
}