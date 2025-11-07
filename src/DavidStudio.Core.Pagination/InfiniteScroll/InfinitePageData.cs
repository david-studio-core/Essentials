using System.Text.Json.Serialization;

namespace DavidStudio.Core.Pagination.InfiniteScroll;

/// <summary>
/// Represents a paginated result set for infinite scroll (cursor) pagination.
/// Contains the retrieved entities and information for fetching the next page.
/// </summary>
/// <typeparam name="T">The type of the items in the result set.</typeparam>
public record InfinitePageData<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InfinitePageData{T}"/> record.
    /// This constructor is used for JSON deserialization.
    /// </summary>
    [JsonConstructor]
    public InfinitePageData() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="InfinitePageData{T}"/> record with the specified entities and cursor information.
    /// </summary>
    /// <param name="entities">The entities retrieved for the current page.</param>
    /// <param name="nextCursor">The dynamic cursor to be used in the next query.</param>
    /// <param name="hasNextPage">Indicates whether there are more pages available after this one.</param>
    public InfinitePageData(IEnumerable<T>? entities, DynamicCursor? nextCursor, bool hasNextPage)
    {
        Entities = entities;
        NextCursor = nextCursor;
        if (nextCursor != null)
            CursorToken = nextCursor.Encode();
        HasNextPage = hasNextPage;
    }

    /// <summary>
    /// Gets the entities retrieved for the current page.
    /// </summary>
    public IEnumerable<T>? Entities { get; private init; }

    /// <summary>
    /// Gets the dynamic cursor to be used in the next query.
    /// </summary>
    public DynamicCursor? NextCursor { get; private init; }

    /// <summary>
    /// Gets the encoded string token representing the <see cref="NextCursor"/>, suitable for use in the next page request.
    /// </summary>
    public string? CursorToken { get; private init; }

    /// <summary>
    /// Gets a value indicating whether there are more pages available after this page.
    /// </summary>
    public bool HasNextPage { get; private init; }
}