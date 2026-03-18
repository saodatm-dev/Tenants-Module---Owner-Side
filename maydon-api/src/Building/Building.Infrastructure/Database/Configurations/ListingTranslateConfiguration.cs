using Building.Domain.Listings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Building.Infrastructure.Database.Configurations;

internal sealed class ListingTranslateConfiguration : IEntityTypeConfiguration<ListingTranslate>
{
	public void Configure(EntityTypeBuilder<ListingTranslate> builder)
	{
		builder.HasIndex(item => item.ListingId);

		builder.HasIndex(item => item.LanguageShortCode);
	}
}
