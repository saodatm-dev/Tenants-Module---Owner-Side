using Building.Domain.RoomTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Building.Infrastructure.Database.Configurations;

internal sealed class RoomTypeConfiguration : IEntityTypeConfiguration<RoomType>
{
	public void Configure(EntityTypeBuilder<RoomType> builder)
	{
		builder.HasIndex(item => item.Id).IsUnique();
	}
}
