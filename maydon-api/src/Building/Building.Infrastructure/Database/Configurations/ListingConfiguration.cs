using System.Text.Json;
using Building.Domain.Listings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Building.Infrastructure.Database.Configurations;

internal sealed class ListingConfiguration : IEntityTypeConfiguration<Listing>
{
	public void Configure(EntityTypeBuilder<Listing> builder)
	{
		builder.HasIndex(item => item.Id).IsUnique();

		builder.HasIndex(item => item.OwnerId);

		builder.Property(item => item.CategoryIds).HasConversion(
			v => JsonSerializer.Serialize(v),
			v => JsonSerializer.Deserialize<List<Guid>>(v));

		builder.Property(item => item.FloorIds).HasConversion(
			v => JsonSerializer.Serialize(v),
			v => JsonSerializer.Deserialize<List<Guid>?>(v));

		builder.Property(item => item.RoomIds).HasConversion(
			v => JsonSerializer.Serialize(v),
			v => JsonSerializer.Deserialize<List<Guid>?>(v));

		builder.Property(item => item.UnitIds).HasConversion(
			v => JsonSerializer.Serialize(v),
			v => JsonSerializer.Deserialize<List<Guid>?>(v));

		builder.Property(item => item.FloorNumbers).HasConversion(
			v => JsonSerializer.Serialize(v),
			v => JsonSerializer.Deserialize<List<short>>(v));

		builder.Property(item => item.Status)
			.HasConversion<int>();

		builder.Property(item => item.ModerationStatus)
			.HasConversion<int>();

		builder.Property(item => item.MinLeaseTerm)
			.HasConversion<int?>();

		builder.Property(item => item.UtilityPaymentType)
			.HasConversion<int?>();

		builder.HasIndex(item => item.RentalPurposeId);
	}
}

