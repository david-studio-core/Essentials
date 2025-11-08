using DavidStudio.Core.DataIO.Repositories;
using DavidStudio.Core.Essentials.CompleteSample.Entities;
using DavidStudio.Core.Essentials.CompleteSample.StronglyTypedIds;

namespace DavidStudio.Core.Essentials.CompleteSample.Repositories;

public interface IManufacturersRepository :
    IBaseRepository<Manufacturer, ManufacturerId>,
    IBaseAggregationRepository<Manufacturer>;
