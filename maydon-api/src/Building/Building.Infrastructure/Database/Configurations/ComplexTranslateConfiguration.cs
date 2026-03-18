using Building.Domain.Complexes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Building.Infrastructure.Database.Configurations;

internal sealed class ComplexTranslateConfiguration : IEntityTypeConfiguration<ComplexTranslate>
{
	public void Configure(EntityTypeBuilder<ComplexTranslate> builder)
	{
		builder.HasIndex(item => item.ComplexId);

		builder.HasIndex(item => item.LanguageShortCode);
	}
}
