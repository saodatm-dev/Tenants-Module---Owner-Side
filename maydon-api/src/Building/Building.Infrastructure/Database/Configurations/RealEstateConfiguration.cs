using Building.Domain.RealEstates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Building.Infrastructure.Database.Configurations;

internal sealed class RealEstateConfiguration : IEntityTypeConfiguration<RealEstate>
{
	public void Configure(EntityTypeBuilder<RealEstate> builder)
	{
		builder.HasIndex(item => item.Id).IsUnique();

		builder.HasIndex(item => item.RealEstateTypeId);

		builder.HasIndex(item => item.RegionId);

		builder.HasIndex(item => item.DistrictId);
	}
}
