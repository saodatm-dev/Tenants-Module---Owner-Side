using Building.Domain.RealEstateTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Building.Infrastructure.Database.Configurations;

internal sealed class RealEstateTypeTranslateConfiguration : IEntityTypeConfiguration<RealEstateTypeTranslate>
{
	public void Configure(EntityTypeBuilder<RealEstateTypeTranslate> builder)
	{
		builder.HasIndex(item => item.RealEstateTypeId);

		builder.HasIndex(item => item.Field);

		builder.HasIndex(item => item.LanguageShortCode);

		builder.Property(item => item.Field)
			.IsRequired()
			.HasConversion<int>();
	}
}
