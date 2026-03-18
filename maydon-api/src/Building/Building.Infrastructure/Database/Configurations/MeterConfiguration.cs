using Building.Domain.Meters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Building.Infrastructure.Database.Configurations;

internal sealed class MeterConfiguration : IEntityTypeConfiguration<Meter>
{
	public void Configure(EntityTypeBuilder<Meter> builder)
	{
		builder.HasIndex(item => item.Id).IsUnique();

		builder.HasIndex(item => item.RealEstateId);

		builder.HasIndex(item => item.SerialNumber);
	}
}
