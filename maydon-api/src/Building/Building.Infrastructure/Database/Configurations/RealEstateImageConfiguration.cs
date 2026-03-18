using Building.Domain.RealEstateImages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Building.Infrastructure.Database.Configurations;

internal sealed class RealEstateImageConfiguration : IEntityTypeConfiguration<RealEstateImage>
{
	public void Configure(EntityTypeBuilder<RealEstateImage> builder)
	{
		builder.HasIndex(item => item.RealEstateId);
	}
}
