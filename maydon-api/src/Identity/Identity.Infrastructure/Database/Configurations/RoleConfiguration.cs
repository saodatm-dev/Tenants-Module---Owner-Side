using Identity.Domain.Roles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Infrastructure.Database.Configurations;

internal sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
	public void Configure(EntityTypeBuilder<Role> builder)
	{
		builder.HasKey(item => item.Id);

		builder.HasIndex(item => item.Id).IsUnique();

		builder.HasIndex(item => item.TenantId);

		builder.Property(item => item.Type)
			.HasConversion<short>();

		builder.HasIndex(item => item.IsActive);
	}
}
