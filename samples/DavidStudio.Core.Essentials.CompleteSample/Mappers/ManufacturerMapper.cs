using DavidStudio.Core.Essentials.CompleteSample.Dtos.Manufacturer;
using DavidStudio.Core.Essentials.CompleteSample.Entities;

namespace DavidStudio.Core.Essentials.CompleteSample.Mappers;

public static class ManufacturerMapper
{
    public static ManufacturerReadDto ToReadDto(this Manufacturer manufacturer)
    {
        return new ManufacturerReadDto(
            manufacturer.Id,
            manufacturer.Name,
            manufacturer.IncorporationDateUtc
        );
    }
}