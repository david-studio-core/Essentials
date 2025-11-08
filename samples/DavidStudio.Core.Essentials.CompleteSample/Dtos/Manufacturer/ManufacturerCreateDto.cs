using System.ComponentModel.DataAnnotations;

namespace DavidStudio.Core.Essentials.CompleteSample.Dtos.Manufacturer;

public record ManufacturerCreateDto
{
    [Length(1, 100)]
    public required string Name { get; init; }
    
    public required DateTime IncorporationDateUtc { get; init; }
}