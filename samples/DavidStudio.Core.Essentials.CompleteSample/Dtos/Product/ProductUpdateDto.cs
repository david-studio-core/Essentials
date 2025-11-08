using System.ComponentModel.DataAnnotations;

namespace DavidStudio.Core.Essentials.CompleteSample.Dtos.Product;

public record ProductUpdateDto
{
    [Length(1, 100)]
    public required string Name { get; init; }

    [Range(0, double.MaxValue)]
    public required decimal Price { get; init; }

    [Range(0, int.MaxValue)]
    public required int StockCount { get; init; }
}