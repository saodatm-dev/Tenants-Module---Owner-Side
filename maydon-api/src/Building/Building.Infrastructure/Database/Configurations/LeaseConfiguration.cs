using Building.Domain.Leases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Building.Infrastructure.Database.Configurations;

internal sealed class LeaseConfiguration : IEntityTypeConfiguration<Lease>
{
	public void Configure(EntityTypeBuilder<Lease> builder)
	{
		builder.HasIndex(item => item.Id).IsUnique();

		builder.HasIndex(item => new { item.OwnerId, item.ClientId });

		builder.HasIndex(item => new { item.AgentId, item.ClientId });

		builder.HasMany(item => item.Items)
			.WithOne(item => item.Lease)
			.HasForeignKey(item => item.LeaseId)
			.OnDelete(DeleteBehavior.Cascade);
	}
}
