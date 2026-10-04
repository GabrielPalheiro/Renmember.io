using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Renmember.Domain.Users;

namespace Renmember.Infrastructure.Persistence.Configurations;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(user => user.Id);
        builder.Property(user => user.Id).ValueGeneratedNever();
        builder.Property(user => user.Name).HasMaxLength(User.NameMaxLength).IsRequired();
        builder.Property(user => user.TimeZone).HasMaxLength(User.TimeZoneMaxLength).IsRequired();
    }
}
