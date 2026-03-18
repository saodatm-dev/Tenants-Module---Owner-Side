using Building.Domain.RealEstateDelegations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Building.Infrastructure.Database.Configurations;

internal sealed class RealEstateDelegationConfiguration : IEntityTypeConfiguration<RealEstateDelegation>
{
	public void Configure(EntityTypeBuilder<RealEstateDelegation> builder)
	{
		builder.HasIndex(item => new { item.OwnerId, item.AgentId });
	}
}
