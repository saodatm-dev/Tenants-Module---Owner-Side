using Core.Domain.Permissions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Infrastructure.Database.Configurations;

public sealed class PermissionTranslateConfiguration : IEntityTypeConfiguration<PermissionTranslate>
{
	public void Configure(EntityTypeBuilder<PermissionTranslate> builder)
	{
		builder.HasKey(item => item.Id);

		builder.HasIndex(item => item.Value);

		builder.HasIndex(item => item.LanguageShortCode);
	}
}
