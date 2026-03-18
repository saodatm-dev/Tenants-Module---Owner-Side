using Building.Domain.LandCategories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Building.Infrastructure.Database.Configurations;

internal sealed class LandCategoryConfiguration : IEntityTypeConfiguration<LandCategory>
{
	public void Configure(EntityTypeBuilder<LandCategory> builder)
	{
		builder.HasKey(item => item.Id);
		builder.HasMany(item => item.Translates)
			.WithOne(item => item.LandCategory)
			.HasForeignKey(item => item.LandCategoryId);
	}
}
