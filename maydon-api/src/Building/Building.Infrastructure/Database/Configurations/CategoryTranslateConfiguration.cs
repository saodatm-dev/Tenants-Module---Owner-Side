using Building.Domain.Categories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Building.Infrastructure.Database.Configurations;

internal sealed class CategoryTranslateConfiguration : IEntityTypeConfiguration<CategoryTranslate>
{
	public void Configure(EntityTypeBuilder<CategoryTranslate> builder)
	{
		builder.HasIndex(item => item.CategoryId);

		builder.HasIndex(item => item.LanguageShortCode);
	}
}
