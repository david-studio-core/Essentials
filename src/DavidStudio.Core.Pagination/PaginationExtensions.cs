namespace DavidStudio.Core.Pagination;

/// <summary>
/// Provides extension methods for pagination.
/// </summary>
public static class PaginationExtensions
{
    /// <summary>
    /// Converts a collection of items into a <see cref="PageData{T}"/> based on the specified pagination options.
    /// </summary>
    /// <typeparam name="T">The type of items in the collection.</typeparam>
    /// <param name="source">The source collection to paginate.</param>
    /// <param name="pageOptions">The pagination options containing page number and page size.</param>
    /// <returns>A <see cref="PageData{T}"/> object containing the paginated items and metadata.</returns>
    public static PageData<T> ToPageData<T>(this IEnumerable<T> source, PageOptions pageOptions)
    {
        var sourceList = source.ToList();

        var entities = sourceList
            .Skip((pageOptions.Page - 1) * pageOptions.Size)
            .Take(pageOptions.Size)
            .ToList();

        var totalCount = sourceList.Count;

        return new PageData<T>(entities, totalCount, pageOptions);
    }
}