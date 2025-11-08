using DavidStudio.Core.DataIO.Services;
using DavidStudio.Core.Essentials.CompleteSample.Dtos.Manufacturer;
using DavidStudio.Core.Essentials.CompleteSample.Entities;
using DavidStudio.Core.Essentials.CompleteSample.StronglyTypedIds;

namespace DavidStudio.Core.Essentials.CompleteSample.Services;

public interface IManufacturersService : IBaseService<Manufacturer, ManufacturerId, ManufacturerCreateDto,
    ManufacturerUpdateDto, ManufacturerReadDto>;