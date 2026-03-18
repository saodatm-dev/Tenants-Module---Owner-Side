using Building.Domain.WishlistItems;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Building.Infrastructure.Database.Configurations;

internal sealed class WishlistItemConfiguration : IEntityTypeConfiguration<WishlistItem>
{
	public void Configure(EntityTypeBuilder<WishlistItem> builder) =>
		builder.HasIndex(item => new { item.WishlistId, item.RealEstateId });
}
