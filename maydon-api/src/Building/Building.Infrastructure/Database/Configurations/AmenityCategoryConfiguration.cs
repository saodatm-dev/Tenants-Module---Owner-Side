using Building.Domain.AmenityCategories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Building.Infrastructure.Database.Configurations;

internal sealed class AmenityCategoryConfiguration : IEntityTypeConfiguration<AmenityCategory>
{
	public void Configure(EntityTypeBuilder<AmenityCategory> builder)
	{
		builder.HasIndex(item => item.Id);
	}
}
