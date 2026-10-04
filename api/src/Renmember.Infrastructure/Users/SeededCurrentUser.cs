using Renmember.Application.Users;

namespace Renmember.Infrastructure.Users;

internal sealed class SeededCurrentUser(SeededUser seededUser) : ICurrentUser
{
    public Guid Id => seededUser.Id;
}
