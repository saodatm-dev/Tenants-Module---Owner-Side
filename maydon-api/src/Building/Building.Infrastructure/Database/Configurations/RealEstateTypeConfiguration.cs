using Building.Domain.RealEstateTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Building.Infrastructure.Database.Configurations;

internal sealed class RealEstateTypeConfiguration : IEntityTypeConfiguration<RealEstateType>
{
	public void Configure(EntityTypeBuilder<RealEstateType> builder)
	{
		builder.HasIndex(item => item.Id).IsUnique();

		builder.HasIndex(item => item.CanHaveUnits);

		builder.HasIndex(item => item.CanHaveMeters);
	}
}
