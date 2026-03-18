using Common.Domain.Districts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Building.Infrastructure.Database.Configurations;

internal sealed class DistrictTranslateConfiguration : IEntityTypeConfiguration<DistrictTranslate>
{
	public void Configure(EntityTypeBuilder<DistrictTranslate> builder)
	{
		builder.ToTable("district_translates", schema: Common.Domain.AssemblyReference.Instance, item => item.ExcludeFromMigrations());
	}
}
