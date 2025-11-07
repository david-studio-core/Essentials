using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DavidStudio.Core.Pagination;

/// <summary>
/// Represents pagination options for a query, including the page number and page size.
/// </summary>
public record PageOptions
{
    /// <summary>
    /// Gets the current page number (1-based).
    /// Must be greater than 0.
    /// </summary>
    [Required]
    [Range(1, int.MaxValue)]
    public int Page { get; }

    /// <summary>
    /// Gets the number of items per page.
    /// Must be between 1 and 100.
    /// </summary>
    [Required]
    [Range(1, 100)]
    public int Size { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PageOptions"/> class.
    /// Parameterless constructor used for deserialization.
    /// </summary>
    [JsonConstructor]
    public PageOptions() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="PageOptions"/> class with the specified page number and size.
    /// </summary>
    /// <param name="page">The current page number (must be greater than 0).</param>
    /// <param name="size">The number of items per page (must be greater than 0).</param>
    /// <exception cref="ArgumentException">
    /// Thrown if <paramref name="page"/> is less than or equal to 0 or
    /// if <paramref name="size"/> is less than or equal to 0.
    /// </exception>
    public PageOptions(int page, int size)
    {
        if (page <= 0)
            throw new ArgumentException(ErrorMessages.PageNumberShouldBeGreaterThanZero, nameof(page));

        if (size <= 0)
            throw new ArgumentException(ErrorMessages.PageSizeShouldBeGreaterThanZero, nameof(size));

        Page = page;
        Size = size;
    }
}