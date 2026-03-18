using Common.Domain.Currencies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Common.Infrastructure.Database.Configurations;

internal sealed class CurrencyConfiguration : IEntityTypeConfiguration<Currency>
{
	public void Configure(EntityTypeBuilder<Currency> builder)
	{
		builder.HasKey(item => item.Id);

		builder.HasIndex(item => item.Code);

		builder.Property(item => item.Order)
			.ValueGeneratedOnAdd();
	}
}
