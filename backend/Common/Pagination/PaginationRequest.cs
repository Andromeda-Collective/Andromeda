namespace Andromeda.Common.Pagination;

public sealed class PaginationRequest
{
    private const int MaxPageSize = 100;
    private int _page = 1;
    private int _pageSize = 20;

    public int Page
    {
        get => _page;
        init => _page = value > 0 ? value : 1;
    }

    public int PageSize
    {
        get => _pageSize;
        init => _pageSize = value is > 0 and <= MaxPageSize ? value : MaxPageSize;
    }
}