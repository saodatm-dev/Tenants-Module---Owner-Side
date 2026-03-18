using Building.Domain.Renovations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Building.Infrastructure.Database.Configurations;

internal sealed class RenovationTranslateConfiguration : IEntityTypeConfiguration<RenovationTranslate>
{
	public void Configure(EntityTypeBuilder<RenovationTranslate> builder)
	{
		builder.HasIndex(item => item.Value);

		builder.HasIndex(item => item.LanguageShortCode);
	}
}
