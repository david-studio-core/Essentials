using DavidStudio.Core.Essentials.CompleteSample.StronglyTypedIds;

namespace DavidStudio.Core.Essentials.CompleteSample.Dtos.Manufacturer;

public record ManufacturerReadDto(
    ManufacturerId Id,
    string Name,
    DateTime IncorporationDateUtc
);