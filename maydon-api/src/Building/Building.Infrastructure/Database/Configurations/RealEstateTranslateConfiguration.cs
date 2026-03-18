using Building.Domain.RealEstates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Building.Infrastructure.Database.Configurations;

internal sealed class RealEstateTranslateConfiguration : IEntityTypeConfiguration<RealEstateTranslate>
{
	public void Configure(EntityTypeBuilder<RealEstateTranslate> builder)
	{
		builder.HasIndex(item => item.RealEstateId);

		builder.HasIndex(item => item.LanguageShortCode);
	}
}
