using Building.Domain.BuildingImages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Building.Infrastructure.Database.Configurations;

internal sealed class BuildingImageConfiguration : IEntityTypeConfiguration<BuildingImage>
{
	public void Configure(EntityTypeBuilder<BuildingImage> builder)
	{
		builder.HasIndex(item => item.BuildingId);
	}
}
