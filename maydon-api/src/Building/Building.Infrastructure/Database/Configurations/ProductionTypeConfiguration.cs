using Building.Domain.ProductionTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Building.Infrastructure.Database.Configurations;

internal sealed class ProductionTypeConfiguration : IEntityTypeConfiguration<ProductionType>
{
	public void Configure(EntityTypeBuilder<ProductionType> builder)
	{
		builder.HasKey(item => item.Id);
		builder.HasMany(item => item.Translates)
			.WithOne(item => item.ProductionType)
			.HasForeignKey(item => item.ProductionTypeId);
	}
}
