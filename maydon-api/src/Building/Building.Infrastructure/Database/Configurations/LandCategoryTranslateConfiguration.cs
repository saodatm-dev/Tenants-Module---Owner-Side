using Building.Domain.LandCategories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Building.Infrastructure.Database.Configurations;

internal sealed class LandCategoryTranslateConfiguration : IEntityTypeConfiguration<LandCategoryTranslate>
{
	public void Configure(EntityTypeBuilder<LandCategoryTranslate> builder)
	{
		builder.HasKey(item => item.Id);
		builder.HasOne(item => item.LandCategory)
			.WithMany(item => item.Translates)
			.HasForeignKey(item => item.LandCategoryId);
	}
}
