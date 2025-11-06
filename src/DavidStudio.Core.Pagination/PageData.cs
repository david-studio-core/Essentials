using System.Text.Json.Serialization;

namespace DavidStudio.Core.Pagination;

public record PageData<T>
{
    [JsonConstructor]
    public PageData() { }

    public PageData(IEnumerable<T>? entities, int totalCount, int page, int size)
    {
        Entities = entities;
        TotalCount = totalCount;
        TotalPages = (int)Math.Ceiling(TotalCount / (double)size);
        HasPreviousPage = page > 1;
        HasNextPage = page < TotalPages;
    }

    public PageData(IEnumerable<T>? entities, int totalCount, PageOptions options)
        : this(entities, totalCount, options.Page, options.Size) { }

    public IEnumerable<T>? Entities { get; protected init; }
    public int TotalCount { get; protected init; }
    public int TotalPages { get; protected init; }
    public bool HasPreviousPage { get; protected init; }
    public bool HasNextPage { get; protected init; }
}