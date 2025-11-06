namespace DavidStudio.Core.Pagination;

public static class PaginationExtensions
{
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