using Building.Domain.ListingCategories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Building.Infrastructure.Database.Configurations;

internal sealed class ListingCategoryConfiguration : IEntityTypeConfiguration<ListingCategory>
{
	public void Configure(EntityTypeBuilder<ListingCategory> builder)
	{
		builder.HasIndex(item => item.Id).IsUnique();

		builder.Property(item => item.Order)
			.ValueGeneratedOnAdd();
	}
}
