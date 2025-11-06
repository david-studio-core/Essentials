using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DavidStudio.Core.Pagination;

public record PageOptions
{
    [Required]
    [Range(1, int.MaxValue)]
    public int Page { get; }

    [Required]
    [Range(1, 100)]
    public int Size { get; }

    [JsonConstructor]
    public PageOptions() { }

    public PageOptions(int page, int size)
    {
        if (page <= 0)
            throw new ArgumentException(nameof(ErrorMessages.PageNumberShouldBeGreaterThanZero), nameof(page));

        if (size <= 0)
            throw new ArgumentException(nameof(ErrorMessages.PageSizeShouldBeGreaterThanZero), nameof(size));

        Page = page;
        Size = size;
    }
}