using Building.Domain.MeterTariffs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Building.Infrastructure.Database.Configurations;

internal sealed class MeterTariffConfiguration : IEntityTypeConfiguration<MeterTariff>
{
	public void Configure(EntityTypeBuilder<MeterTariff> builder)
	{
		builder.HasIndex(item => item.MeterTypeId);
	}
}
