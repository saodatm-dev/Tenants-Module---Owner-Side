using Building.Domain.MeterTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Building.Infrastructure.Database.Configurations;

internal sealed class MeterTypeConfiguration : IEntityTypeConfiguration<MeterType>
{
	public void Configure(EntityTypeBuilder<MeterType> builder)
	{
		builder.HasIndex(item => item.Id).IsUnique();
	}
}
