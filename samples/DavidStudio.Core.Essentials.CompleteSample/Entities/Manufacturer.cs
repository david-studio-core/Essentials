using DavidStudio.Core.DataIO.Entities;
using DavidStudio.Core.Essentials.CompleteSample.Dtos.Manufacturer;
using DavidStudio.Core.Essentials.CompleteSample.StronglyTypedIds;
using MassTransit;

namespace DavidStudio.Core.Essentials.CompleteSample.Entities;

public sealed class Manufacturer : Entity<ManufacturerId>,
    ISelfManageable<Manufacturer, ManufacturerCreateDto, ManufacturerUpdateDto>
{
    private Manufacturer() { }

    public string Name { get; private set; } = null!;
    public DateTime IncorporationDateUtc { get; private init; }

    public static Manufacturer Create(ManufacturerCreateDto model)
    {
        return new Manufacturer
        {
            Id = new ManufacturerId(NewId.NextGuid()),
            Name = model.Name,
            IncorporationDateUtc = model.IncorporationDateUtc
        };
    }

    public void Update(ManufacturerUpdateDto model)
    {
        Name = model.Name;
    }
}