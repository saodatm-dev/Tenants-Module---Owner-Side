using Building.Domain.Amenities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Building.Infrastructure.Database.Configurations;

internal sealed class AmenityConfiguration : IEntityTypeConfiguration<Amenity>
{
	public void Configure(EntityTypeBuilder<Amenity> builder)
	{
		builder.HasIndex(item => item.AmenityCategoryId);
	}
}
