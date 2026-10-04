using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Renmember.Infrastructure.Persistence;

namespace Renmember.Infrastructure;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<RenmemberDbContext>(options => options.UseNpgsql(connectionString));

        return services;
    }

    public static async Task AplicarMigrationsAsync(this IServiceProvider services, CancellationToken cancellationToken = default)
    {
        await using var scope = services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<RenmemberDbContext>();

        await dbContext.Database.MigrateAsync(cancellationToken);
    }
}
