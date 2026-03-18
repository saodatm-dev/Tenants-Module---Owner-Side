using Common.Domain.Banks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Common.Infrastructure.Database.Configurations;

internal sealed class BankConfiguration : IEntityTypeConfiguration<Bank>
{
	public void Configure(EntityTypeBuilder<Bank> builder)
	{
		builder.HasKey(item => item.Id);

		builder.HasIndex(item => item.Mfo);

		builder.Property(item => item.Order)
			.ValueGeneratedOnAdd();
	}
}
