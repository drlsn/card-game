using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Trinica.Api;

public class ApiHealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(HealthCheckResult.Healthy("A healthy result."));
    }
}
