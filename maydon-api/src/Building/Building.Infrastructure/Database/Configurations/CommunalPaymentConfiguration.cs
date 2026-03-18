using Building.Domain.CommunalBills;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Building.Infrastructure.Database.Configurations;

internal sealed class CommunalPaymentConfiguration : IEntityTypeConfiguration<CommunalPayment>
{
	public void Configure(EntityTypeBuilder<CommunalPayment> builder)
	{
		builder.HasIndex(item => item.CommunalBillId);
	}
}
