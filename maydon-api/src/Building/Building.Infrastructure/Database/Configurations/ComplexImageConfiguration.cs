using Building.Domain.ComplexImages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Building.Infrastructure.Database.Configurations;

internal sealed class ComplexImageConfiguration : IEntityTypeConfiguration<ComplexImage>
{
	public void Configure(EntityTypeBuilder<ComplexImage> builder)
	{
		builder.HasIndex(item => item.ComplexId);
	}
}
