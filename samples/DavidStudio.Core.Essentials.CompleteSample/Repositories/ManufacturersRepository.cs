using DavidStudio.Core.DataIO.Repositories;
using DavidStudio.Core.Essentials.CompleteSample.Database;
using DavidStudio.Core.Essentials.CompleteSample.Entities;
using DavidStudio.Core.Essentials.CompleteSample.StronglyTypedIds;
using Microsoft.EntityFrameworkCore;

namespace DavidStudio.Core.Essentials.CompleteSample.Repositories;

public class ManufacturersRepository(ApplicationDbContext context) 
    : BaseRepository<Manufacturer, ManufacturerId>(context),
        IManufacturersRepository;