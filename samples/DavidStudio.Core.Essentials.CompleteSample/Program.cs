using DavidStudio.Core.Auth.Conventions;
using DavidStudio.Core.Auth.Extensions;
using DavidStudio.Core.DataIO.Extensions;
using DavidStudio.Core.Essentials.CompleteSample.Database;
using DavidStudio.Core.Essentials.CompleteSample.Extensions;
using DavidStudio.Core.Swagger.Extensions;
using DavidStudio.Core.Utilities.Extensions;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilogFromConfiguration();

builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddAuthorization();

#if DEBUG
builder.Services.AddPermissionAuthorizationStub();
#else
builder.Services.AddPermissionAuthorization();
#endif

builder.Services.AddDefaultApiVersioning();

builder.Services
    .AddControllers(options => options.Conventions.Add(new UnauthorizedResponseConvention()))
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter()));

var sqlConnectionString = builder.Configuration.GetConnectionString("CompleteSampleDb");

builder.Services.AddDatabase<ApplicationDbContext>(
    sqlConnectionString,
    typeof(ApplicationDbContext).Assembly.GetName().Name
);

builder.Services.AddEfUnitOfWork<ApplicationDbContext>();

builder.Services.AddRepositories();
builder.Services.AddServices();

#if DEBUG
if (builder.Configuration["USE_REAL_EVENTBUS"] != "1")
    builder.Services.AddInMemoryEventBus();
else
    builder.Services.AddRabbitMq("EventBus");
#else
builder.Services.AddRabbitMq("EventBus");
#endif

builder.Services.AddSwaggerWithBearer("Complete Sample API");

builder.Services.AddDefaultOpenTelemetry("CompleteSampleApi");

builder.Services.AddHealthChecks()
    .AddSqlServer(sqlConnectionString!, tags: ["ready", "sql"]);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDefaultSwagger();

    app.Services.MigrateDatabase<ApplicationDbContext>();
}

app.UseSerilogRequestLogging();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers().RequireAuthorization();

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.Run();