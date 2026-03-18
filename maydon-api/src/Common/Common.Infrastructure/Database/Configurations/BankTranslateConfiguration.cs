using Common.Domain.Banks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Common.Infrastructure.Database.Configurations;

internal sealed class BankTranslateConfiguration : IEntityTypeConfiguration<BankTranslate>
{
	public void Configure(EntityTypeBuilder<BankTranslate> builder)
	{
		builder.HasKey(item => item.Id);

		builder.HasIndex(item => item.Value);

		builder.HasIndex(item => item.LanguageShortCode);
	}
}
