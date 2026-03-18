using Building.Domain.Wishlists;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Building.Infrastructure.Database.Configurations;

internal sealed class WishlistConfiguration : IEntityTypeConfiguration<Wishlist>
{
	public void Configure(EntityTypeBuilder<Wishlist> builder)
	{
		builder.HasIndex(item => new { item.TenantId, item.UserId });
	}
}
