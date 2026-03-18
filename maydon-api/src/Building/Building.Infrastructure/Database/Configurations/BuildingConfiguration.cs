using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Building.Infrastructure.Database.Configurations;

internal sealed class BuildingConfiguration : IEntityTypeConfiguration<Domain.Buildings.Building>
{
	public void Configure(EntityTypeBuilder<Domain.Buildings.Building> builder)
	{
		builder.HasIndex(item => item.Id).IsUnique();

		builder.Property(item => item.Status)
			.HasConversion<int>();
	}
}
