using Building.Domain.ListingRequests;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Building.Infrastructure.Database.Configurations;

internal sealed class ListingRequestConfiguration : IEntityTypeConfiguration<ListingRequest>
{
	public void Configure(EntityTypeBuilder<ListingRequest> builder)
	{
		builder.HasIndex(item => item.Id).IsUnique();

		builder.HasIndex(item => item.OwnerId);

		builder.HasIndex(item => item.ClientId);

		builder.Property(item => item.Status)
			.HasConversion<int>();
	}
}
