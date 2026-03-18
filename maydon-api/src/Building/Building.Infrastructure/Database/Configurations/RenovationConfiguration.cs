using Building.Domain.Renovations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Building.Infrastructure.Database.Configurations;

internal sealed class RenovationConfiguration : IEntityTypeConfiguration<Renovation>
{
	public void Configure(EntityTypeBuilder<Renovation> builder)
	{
		builder.HasIndex(item => item.IsActive);
	}
}
