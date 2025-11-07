using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Impl.AdoJobStore;

namespace DavidStudio.Core.Utilities.Extensions;

public static class QuartzExtensions
{
    public static IServiceCollection AddQuartzDefaults(this IServiceCollection services,
        string? connectionString,
        params QuartzJobRegistration[] jobs)
    {
        ArgumentNullException.ThrowIfNull(connectionString);

        services.AddQuartz(quartz =>
        {
            quartz.UsePersistentStore(store =>
            {
                store.UseProperties = false;

                store.UseNewtonsoftJsonSerializer();

                store.UseSqlServer(sqlServerOptions =>
                {
                    sqlServerOptions.UseDriverDelegate<SqlServerDelegate>();
                    sqlServerOptions.ConnectionString = connectionString;
                    sqlServerOptions.TablePrefix = "quartz.QRTZ_";
                });

                store.UseClustering();
            });

            foreach (var job in jobs)
                job(quartz);
        });

        services.AddQuartzHostedService(opt => { opt.WaitForJobsToComplete = true; });

        return services;
    }

    public delegate void QuartzJobRegistration(IServiceCollectionQuartzConfigurator quartz);

    public static void AddCronJobAndTrigger<T>(this IServiceCollectionQuartzConfigurator quartz,
        string? cronSchedule,
        Action<CronScheduleBuilder>? action = null)
        where T : IJob
    {
        ArgumentNullException.ThrowIfNull(cronSchedule);

        var jobName = typeof(T).Name;
        var jobKey = new JobKey(jobName);

        quartz.AddJob<T>(opts => opts.WithIdentity(jobKey));

        quartz.AddTrigger(opts => opts
            .ForJob(jobKey)
            .WithIdentity(jobName + "-trigger")
            .WithCronSchedule(cronSchedule, action)
        );
    }
}