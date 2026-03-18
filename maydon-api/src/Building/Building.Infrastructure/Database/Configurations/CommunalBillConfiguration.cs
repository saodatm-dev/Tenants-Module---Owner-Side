using Building.Domain.CommunalBills;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Building.Infrastructure.Database.Configurations;

internal sealed class CommunalBillConfiguration : IEntityTypeConfiguration<CommunalBill>
{
	public void Configure(EntityTypeBuilder<CommunalBill> builder)
	{
		builder.HasIndex(item => item.RealEstateId);

		builder.HasIndex(item => item.MeterReadingId);

		builder.HasIndex(item => item.MeterTariffId);

		builder.HasIndex(item => item.Status);

		builder.HasMany(item => item.Payments)
			.WithOne(item => item.CommunalBill)
			.HasForeignKey(item => item.CommunalBillId)
			.OnDelete(DeleteBehavior.Cascade);
	}
}
