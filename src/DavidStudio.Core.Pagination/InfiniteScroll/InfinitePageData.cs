using System.Text.Json.Serialization;

namespace DavidStudio.Core.Pagination.InfiniteScroll;

public record InfinitePageData<T>
{
    [JsonConstructor]
    public InfinitePageData() { }

    public InfinitePageData(IEnumerable<T>? entities, DynamicCursor? lastCursor, bool hasNextPage)
    {
        Entities = entities;
        LastCursor = lastCursor;
        if (lastCursor != null)
            CursorToken = lastCursor.Encode();
        HasNextPage = hasNextPage;
    }

    public IEnumerable<T>? Entities { get; private init; }
    public DynamicCursor? LastCursor { get; private init; }
    public string? CursorToken { get; private init; }
    public bool HasNextPage { get; private init; }
}