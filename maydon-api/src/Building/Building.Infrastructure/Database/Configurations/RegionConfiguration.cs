using Common.Domain.Regions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Building.Infrastructure.Database.Configurations;

internal sealed class RegionConfiguration : IEntityTypeConfiguration<Region>
{
	public void Configure(EntityTypeBuilder<Region> builder)
	{
		builder.ToTable("regions", schema: Common.Domain.AssemblyReference.Instance, item => item.ExcludeFromMigrations());
	}
}
