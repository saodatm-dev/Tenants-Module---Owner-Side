using Building.Domain.MeterTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Building.Infrastructure.Database.Configurations;

internal sealed class MeterTypeTranslateConfiguration : IEntityTypeConfiguration<MeterTypeTranslate>
{
	public void Configure(EntityTypeBuilder<MeterTypeTranslate> builder)
	{
		builder.HasIndex(item => item.MeterTypeId);

		builder.HasIndex(item => item.Field);

		builder.HasIndex(item => item.LanguageShortCode);

		builder.Property(item => item.Field)
			.HasConversion<int>();
	}
}
