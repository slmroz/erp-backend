namespace ERP.Services.Abstractions.Search;
public sealed class PagedResult<T>
{
    public int TotalCount { get; init; }
    public IReadOnlyList<T> Items { get; init; } = [];
}