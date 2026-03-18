using Common.Domain.Districts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Common.Infrastructure.Database.Configurations;

internal sealed class DistrictConfiguration : IEntityTypeConfiguration<District>
{
	public void Configure(EntityTypeBuilder<District> builder)
	{
		builder.HasKey(item => item.Id);

		builder.HasIndex(item => item.RegionId);

		builder.Property(item => item.Order)
			.ValueGeneratedOnAdd();
	}
}
