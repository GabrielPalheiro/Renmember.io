using Microsoft.Extensions.Diagnostics.HealthChecks;
using Renmember.Infrastructure.Persistence;

namespace Renmember.Api.Health;

internal sealed class DatabaseHealthCheck(RenmemberDbContext dbContext) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default) =>
        await dbContext.Database.CanConnectAsync(cancellationToken)
            ? HealthCheckResult.Healthy()
            : HealthCheckResult.Unhealthy("Sem conexão com o banco.");
}
