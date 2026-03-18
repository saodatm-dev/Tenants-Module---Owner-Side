using Common.Domain.Districts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Common.Infrastructure.Database.Configurations;

internal sealed class DistrictTranslateConfiguration : IEntityTypeConfiguration<DistrictTranslate>
{
	public void Configure(EntityTypeBuilder<DistrictTranslate> builder)
	{
		builder.HasKey(item => item.Id);

		builder.HasIndex(item => item.DistrictId);

		builder.HasIndex(item => item.LanguageShortCode);
	}
}
