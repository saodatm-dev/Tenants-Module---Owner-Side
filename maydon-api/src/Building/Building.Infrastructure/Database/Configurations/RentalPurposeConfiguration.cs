using Building.Domain.RentalPurposes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Building.Infrastructure.Database.Configurations;

internal sealed class RentalPurposeConfiguration : IEntityTypeConfiguration<RentalPurpose>
{
	public void Configure(EntityTypeBuilder<RentalPurpose> builder)
	{
		builder.HasIndex(item => item.Id).IsUnique();
	}
}
