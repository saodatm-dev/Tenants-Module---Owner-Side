using Building.Domain.RoomTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Building.Infrastructure.Database.Configurations;

internal sealed class RoomTypeTranslateConfiguration : IEntityTypeConfiguration<RoomTypeTranslate>
{
	public void Configure(EntityTypeBuilder<RoomTypeTranslate> builder)
	{
		builder.HasIndex(item => item.Value);

		builder.HasIndex(item => item.LanguageShortCode);
	}
}
