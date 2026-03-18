using Common.Domain.Languages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Common.Infrastructure.Database.Configurations;

internal sealed class LanguageConfiguration : IEntityTypeConfiguration<Language>
{
	public void Configure(EntityTypeBuilder<Language> builder)
	{
		builder.HasKey(item => item.Id);

		builder.Property(item => item.Name)
			.IsRequired();

		builder.Property(item => item.ShortCode)
			.IsRequired();

		builder.HasIndex(item => item.Name);

		builder.HasIndex(item => item.ShortCode);

		builder.Property(item => item.Order)
			.ValueGeneratedOnAdd();
	}
}
