using Building.Domain.MeterReadings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Building.Infrastructure.Database.Configurations;

internal sealed class MeterReadingConfiguration : IEntityTypeConfiguration<MeterReading>
{
	public void Configure(EntityTypeBuilder<MeterReading> builder)
	{
		builder.HasIndex(item => item.MeterId);

		builder.HasIndex(item => item.RealEstateId);
	}
}
