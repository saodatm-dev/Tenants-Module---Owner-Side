using Building.Domain.Floors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Building.Infrastructure.Database.Configurations;

internal sealed class FloorConfiguration : IEntityTypeConfiguration<Floor>
{
	public void Configure(EntityTypeBuilder<Floor> builder)
	{
		builder.HasIndex(item => item.Id).IsUnique();

		builder.HasIndex(item => item.Number);

		builder.HasIndex(item => item.BuildingId);

		builder.HasIndex(item => item.RealEstateId);

		builder.HasIndex(item => item.Type);

		// Floor belongs to a Building (optional)
		builder.HasOne(f => f.Building)
			.WithMany(b => b.Floors)
			.HasForeignKey(f => f.BuildingId)
			.OnDelete(DeleteBehavior.SetNull);

		// Floor belongs to a RealEstate as parent (optional, dual-parent model)
		builder.HasOne(f => f.RealEstate)
			.WithMany()
			.HasForeignKey(f => f.RealEstateId)
			.OnDelete(DeleteBehavior.SetNull);

		// RealEstates that are ON this floor (inverse of RealEstate.FloorId)
		builder.HasMany(f => f.RealEstates)
			.WithOne()
			.HasForeignKey("FloorId")
			.OnDelete(DeleteBehavior.SetNull);
	}
}
