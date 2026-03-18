using Common.Domain.Currencies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Common.Infrastructure.Database.Configurations;

internal sealed class CurrencyTranslateConfiguration : IEntityTypeConfiguration<CurrencyTranslate>
{
	public void Configure(EntityTypeBuilder<CurrencyTranslate> builder)
	{
		builder.HasKey(item => item.Id);

		builder.HasIndex(item => item.CurrencyId);

		builder.HasIndex(item => item.LanguageShortCode);
	}
}
