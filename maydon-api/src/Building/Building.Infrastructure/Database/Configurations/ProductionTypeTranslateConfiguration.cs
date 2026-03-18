using Building.Domain.ProductionTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Building.Infrastructure.Database.Configurations;

internal sealed class ProductionTypeTranslateConfiguration : IEntityTypeConfiguration<ProductionTypeTranslate>
{
	public void Configure(EntityTypeBuilder<ProductionTypeTranslate> builder)
	{
		builder.HasKey(item => item.Id);
		builder.HasOne(item => item.ProductionType)
			.WithMany(item => item.Translates)
			.HasForeignKey(item => item.ProductionTypeId);
	}
}
