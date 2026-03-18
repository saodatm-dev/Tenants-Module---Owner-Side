using Common.Domain.Regions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Common.Infrastructure.Database.Configurations;

internal sealed class RegionTranslateConfiguration : IEntityTypeConfiguration<RegionTranslate>
{
	public void Configure(EntityTypeBuilder<RegionTranslate> builder)
	{
		builder.HasKey(item => item.Id);

		builder.HasIndex(item => item.RegionId);

		builder.HasIndex(item => item.LanguageShortCode);
	}
}
