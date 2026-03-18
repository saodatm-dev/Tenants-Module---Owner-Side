using Building.Domain.AmenityCategories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Building.Infrastructure.Database.Configurations;

internal sealed class AmenityCategoryTranslateConfiguration : IEntityTypeConfiguration<AmenityCategoryTranslate>
{
	public void Configure(EntityTypeBuilder<AmenityCategoryTranslate> builder)
	{
		builder.HasIndex(item => item.AmenityCategoryId);

		builder.HasIndex(item => item.LanguageShortCode);
	}
}
