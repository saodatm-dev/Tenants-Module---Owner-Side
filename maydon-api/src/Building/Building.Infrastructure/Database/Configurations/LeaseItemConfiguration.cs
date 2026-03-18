using Building.Domain.Leases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Building.Infrastructure.Database.Configurations;

internal sealed class LeaseItemConfiguration : IEntityTypeConfiguration<LeaseItem>
{
    public void Configure(EntityTypeBuilder<LeaseItem> builder)
    {
        builder.HasIndex(item => item.Id).IsUnique();

        builder.HasIndex(item => item.LeaseId);

        builder.HasIndex(item => item.ListingId);

        builder.HasIndex(item => item.RealEstateId);
    }
}
