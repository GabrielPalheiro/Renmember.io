using Microsoft.EntityFrameworkCore;
using Renmember.Domain.Users;

namespace Renmember.Infrastructure.Persistence;

public sealed class RenmemberDbContext(DbContextOptions<RenmemberDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(RenmemberDbContext).Assembly);
}
