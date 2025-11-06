using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace DavidStudio.Core.Pagination.InfiniteScroll;

public record InfinitePageOptions
{
    [Required]
    [Range(1, 100)]
    public int Size { get; }

    [MemberNotNullWhen(returnValue: false, nameof(SearchAfterToken))]
    public DynamicCursor? SearchAfter { get; set; }

    [MemberNotNullWhen(returnValue: false, nameof(SearchAfter))]
    public string? SearchAfterToken { get; }

    public InfinitePageOptions(int size, DynamicCursor? searchAfter)
    {
        if (size <= 0)
            throw new ArgumentException(nameof(ErrorMessages.PageSizeShouldBeGreaterThanZero), nameof(size));

        Size = size;
        SearchAfter = searchAfter;
    }

    public InfinitePageOptions(int size, string? searchAfterToken)
    {
        if (size <= 0)
            throw new ArgumentException(nameof(ErrorMessages.PageSizeShouldBeGreaterThanZero), nameof(size));

        Size = size;
        SearchAfterToken = searchAfterToken;
    }
}