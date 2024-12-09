using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Threading;
using System.Threading.Tasks;

public class CustomHealthCheck : IHealthCheck
    {
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
        // Your custom health check logic here
        bool isHealthy = true; // Replace with actual health check logic

        if (isHealthy)
            {
            return Task.FromResult(HealthCheckResult.Healthy("The check indicates a healthy result."));
            }

        return Task.FromResult(HealthCheckResult.Unhealthy("The check indicates an unhealthy result."));
        }
    }