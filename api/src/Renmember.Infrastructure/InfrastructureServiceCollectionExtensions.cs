using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Renmember.Application.Users;
using Renmember.Infrastructure.Persistence;
using Renmember.Infrastructure.Persistence.Seeding;
using Renmember.Infrastructure.Users;

namespace Renmember.Infrastructure;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string connectionString,
        SeededUser seededUser)
    {
        services.AddSingleton(seededUser);
        services.AddScoped<ICurrentUser, SeededCurrentUser>();
        services.AddDbContext<RenmemberDbContext>(options => options
            .UseNpgsql(connectionString)
            .UseAsyncSeeding((dbContext, _, cancellationToken) =>
                SeededUserSeeding.SemearAsync(dbContext, seededUser, cancellationToken)));

        return services;
    }

    public static async Task AplicarMigrationsAsync(this IServiceProvider services, CancellationToken cancellationToken = default)
    {
        await using var scope = services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<RenmemberDbContext>();

        await dbContext.Database.MigrateAsync(cancellationToken);
    }
}
