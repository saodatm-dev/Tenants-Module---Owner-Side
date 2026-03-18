using Core.Domain.Permissions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Building.Infrastructure.Database.Configurations;

public sealed class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
	public void Configure(EntityTypeBuilder<Permission> builder)
	{
		builder.ToTable("permissions", schema: Identity.Domain.AssemblyReference.Instance, item => item.ExcludeFromMigrations());
	}
}
