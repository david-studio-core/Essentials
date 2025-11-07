using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Impl.AdoJobStore;

namespace DavidStudio.Core.Utilities.Extensions;

/// <summary>
/// Provides extension methods for registering and configuring Quartz.NET jobs and triggers with default settings in an ASP.NET Core application.
/// </summary>
public static class QuartzExtensions
{
    /// <summary>
    /// Adds Quartz.NET services with default configuration, including persistent store, clustering, JSON serialization, and hosted service support.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add Quartz services to.</param>
    /// <param name="connectionString">The database connection string for Quartz's persistent job store.</param>
    /// <param name="jobs">An optional list of job registration delegates to configure individual jobs.</param>
    /// <returns>The <see cref="IServiceCollection"/> for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="connectionString"/> is <c>null</c>.</exception>
    /// <remarks>
    /// This method configures Quartz.NET with:
    /// <list type="bullet">
    /// <item>Persistent store using SQL Server with table prefix "quartz.QRTZ_".</item>
    /// <item>Clustering enabled for distributed job execution.</item>
    /// <item>JSON serialization for job data using Newtonsoft.Json.</item>
    /// <item>Automatic registration of jobs and triggers provided via <paramref name="jobs"/> delegates.</item>
    /// <item>A hosted service that waits for jobs to complete on application shutdown.</item>
    /// </list>
    /// </remarks>
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

    /// <summary>
    /// Delegate type for registering Quartz jobs using an <see cref="IServiceCollectionQuartzConfigurator"/>.
    /// </summary>
    /// <param name="quartz">The <see cref="IServiceCollectionQuartzConfigurator"/> used to configure jobs and triggers.</param>
    public delegate void QuartzJobRegistration(IServiceCollectionQuartzConfigurator quartz);

    /// <summary>
    /// Registers a Cron job of type <typeparamref name="T"/> and adds a corresponding trigger based on the specified Cron expression.
    /// </summary>
    /// <typeparam name="T">The type of the job implementing <see cref="IJob"/>.</typeparam>
    /// <param name="quartz">The <see cref="IServiceCollectionQuartzConfigurator"/> used to configure the job.</param>
    /// <param name="cronSchedule">The Cron expression to define the job's schedule.</param>
    /// <param name="action">An optional <see cref="Action{CronScheduleBuilder}"/> to further customize the trigger schedule.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="cronSchedule"/> is <c>null</c>.</exception>
    /// <remarks>
    /// This method automatically:
    /// <list type="bullet">
    /// <item>Creates a <see cref="JobKey"/> based on the job type's name.</item>
    /// <item>Adds the job to the Quartz configuration.</item>
    /// <item>Adds a trigger for the job using the provided Cron schedule.</item>
    /// </list>
    /// </remarks>
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