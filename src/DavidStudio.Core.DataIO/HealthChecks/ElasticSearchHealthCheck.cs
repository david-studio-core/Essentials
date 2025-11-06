using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace DavidStudio.Core.DataIO.HealthChecks;

public class ElasticSearchHealthCheck(ElasticsearchClient client) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await client.PingAsync(cancellationToken: cancellationToken);

            return response.IsValidResponse
                ? HealthCheckResult.Healthy("Elasticsearch is healthy")
                : HealthCheckResult.Unhealthy("Elasticsearch ping failed");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Elasticsearch ping exception", ex);
        }
    }
}