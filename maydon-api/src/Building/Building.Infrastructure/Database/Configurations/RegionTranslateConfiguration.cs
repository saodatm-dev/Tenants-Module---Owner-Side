using Common.Domain.Regions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Building.Infrastructure.Database.Configurations;

internal sealed class RegionTranslateConfiguration : IEntityTypeConfiguration<RegionTranslate>
{
	public void Configure(EntityTypeBuilder<RegionTranslate> builder)
	{
		builder.ToTable("region_translates", schema: Common.Domain.AssemblyReference.Instance, item => item.ExcludeFromMigrations());
	}
}
