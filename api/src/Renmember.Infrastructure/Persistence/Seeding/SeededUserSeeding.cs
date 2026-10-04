using Microsoft.EntityFrameworkCore;
using Renmember.Domain.Users;
using Renmember.Infrastructure.Users;

namespace Renmember.Infrastructure.Persistence.Seeding;

internal static class SeededUserSeeding
{
    public static async Task SemearAsync(DbContext dbContext, SeededUser seededUser, CancellationToken cancellationToken)
    {
        var users = dbContext.Set<User>();

        if (await users.AnyAsync(user => user.Id == seededUser.Id, cancellationToken))
        {
            return;
        }

        users.Add(User.Criar(seededUser.Id, seededUser.Name, seededUser.TimeZone));
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
