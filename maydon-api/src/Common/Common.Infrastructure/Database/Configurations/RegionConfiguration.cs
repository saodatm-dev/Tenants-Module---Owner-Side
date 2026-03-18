using Common.Domain.Regions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Common.Infrastructure.Database.Configurations;

internal sealed class RegionConfiguration : IEntityTypeConfiguration<Region>
{
	public void Configure(EntityTypeBuilder<Region> builder)
	{
		builder.HasKey(item => item.Id);

		builder.Property(item => item.Order)
			.ValueGeneratedOnAdd();
	}
}
