using Building.Domain.Amenities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Building.Infrastructure.Database.Configurations;

internal sealed class AmenityTranslateConfiguration : IEntityTypeConfiguration<AmenityTranslate>
{
	public void Configure(EntityTypeBuilder<AmenityTranslate> builder)
	{
		builder.HasIndex(item => item.AmenityId);

		builder.HasIndex(item => item.LanguageShortCode);
	}
}
