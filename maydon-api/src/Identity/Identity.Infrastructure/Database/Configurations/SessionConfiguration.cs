using Identity.Domain.Sessions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Infrastructure.Database.Configurations;

internal sealed class SessionConfiguration : IEntityTypeConfiguration<Session>
{
	public void Configure(EntityTypeBuilder<Session> builder)
	{
		builder.HasKey(item => item.Id);

		builder.HasIndex(item => item.Id).IsUnique();

		builder.HasIndex(item => item.RefreshToken);

		builder.Property(item => item.RefreshToken)
			.IsRequired();

		builder.Property(item => item.RefreshTokenExpiryTime)
			.IsRequired();
	}
}
