using Building.Domain.ListingCategories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Building.Infrastructure.Database.Configurations;

internal sealed class ListingCategoryTranslateConfiguration : IEntityTypeConfiguration<ListingCategoryTranslate>
{
	public void Configure(EntityTypeBuilder<ListingCategoryTranslate> builder)
	{
		builder.HasIndex(item => item.ListingCategoryId);

		builder.HasIndex(item => item.LanguageShortCode);
	}
}
