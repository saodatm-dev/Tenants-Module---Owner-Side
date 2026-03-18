using Core.Domain.Permissions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Common.Infrastructure.Database.Configurations;

public sealed class PermissionTranslateConfiguration : IEntityTypeConfiguration<PermissionTranslate>
{
	public void Configure(EntityTypeBuilder<PermissionTranslate> builder)
	{
		builder.ToTable("permission_translates", schema: "identity", item => item.ExcludeFromMigrations());
	}
}
