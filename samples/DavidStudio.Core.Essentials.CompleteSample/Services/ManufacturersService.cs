using System.Linq.Expressions;
using DavidStudio.Core.DataIO.Services;
using DavidStudio.Core.DataIO.UnitOfWork.EfCore;
using DavidStudio.Core.Essentials.CompleteSample.Database;
using DavidStudio.Core.Essentials.CompleteSample.Dtos.Manufacturer;
using DavidStudio.Core.Essentials.CompleteSample.Entities;
using DavidStudio.Core.Essentials.CompleteSample.Mappers;
using DavidStudio.Core.Essentials.CompleteSample.Repositories;
using DavidStudio.Core.Essentials.CompleteSample.StronglyTypedIds;

namespace DavidStudio.Core.Essentials.CompleteSample.Services;

public class ManufacturersService(IManufacturersRepository repository, IEfUnitOfWork<ApplicationDbContext> unitOfWork)
    : BaseService<ApplicationDbContext, IManufacturersRepository, Manufacturer, ManufacturerId, ManufacturerCreateDto,
            ManufacturerUpdateDto, ManufacturerReadDto>(repository, unitOfWork),
        IManufacturersService
{
    protected override Expression<Func<Manufacturer, ManufacturerReadDto>> ToReadDto => e => e.ToReadDto();
}