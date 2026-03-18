using Building.Domain.Units;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Building.Infrastructure.Database.Configurations;

internal sealed class RealEstateUnitConfiguration : IEntityTypeConfiguration<Unit>
{
	public void Configure(EntityTypeBuilder<Unit> builder)
	{
		builder.HasIndex(item => item.OwnerId);

		builder.HasIndex(item => item.RealEstateId);

		builder.HasIndex(item => item.RealEstateTypeId);

		builder.Property(item => item.Coordinates)
			.HasColumnType("jsonb")
			.HasConversion(
				v => v != null ? System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null) : null,
				v => v != null ? System.Text.Json.JsonSerializer.Deserialize<List<UnitCoordinate>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new List<UnitCoordinate>() : new List<UnitCoordinate>());
	}
}
