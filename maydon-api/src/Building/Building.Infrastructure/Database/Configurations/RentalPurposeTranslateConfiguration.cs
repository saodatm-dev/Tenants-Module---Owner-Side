using Building.Domain.RentalPurposes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Building.Infrastructure.Database.Configurations;

internal sealed class RentalPurposeTranslateConfiguration : IEntityTypeConfiguration<RentalPurposeTranslate>
{
	public void Configure(EntityTypeBuilder<RentalPurposeTranslate> builder)
	{
		builder.HasIndex(item => item.RentalPurposeId);

		builder.HasIndex(item => item.LanguageShortCode);
	}
}
