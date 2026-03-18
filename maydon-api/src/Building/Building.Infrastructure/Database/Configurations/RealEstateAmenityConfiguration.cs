using Building.Domain.RealEstateAmenities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Building.Infrastructure.Database.Configurations;

internal sealed class RealEstateAmenityConfiguration : IEntityTypeConfiguration<RealEstateAmenity>
{
	public void Configure(EntityTypeBuilder<RealEstateAmenity> builder)
	{
		builder.HasIndex(item => new { item.RealEstateId, item.AmenityId });
	}
}
