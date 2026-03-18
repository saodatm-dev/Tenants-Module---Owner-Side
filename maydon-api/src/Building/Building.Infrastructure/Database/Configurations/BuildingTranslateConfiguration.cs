using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Building.Infrastructure.Database.Configurations;

internal sealed class BuildingTranslateConfiguration : IEntityTypeConfiguration<Domain.Buildings.BuildingTranslate>
{
	public void Configure(EntityTypeBuilder<Domain.Buildings.BuildingTranslate> builder)
	{
		builder.HasIndex(item => item.BuildingId);

		builder.HasIndex(item => item.LanguageShortCode);
	}
}
