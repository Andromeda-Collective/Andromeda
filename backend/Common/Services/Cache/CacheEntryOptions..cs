namespace Andromeda.Common.Services.Cache;

public sealed class CacheEntryOptions
{
    public TimeSpan? AbsoluteExpirationRelativeToNow { get; init; }

    public TimeSpan? SlidingExpiration { get; init; }
}