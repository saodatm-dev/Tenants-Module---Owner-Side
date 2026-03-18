using Building.Domain.Complexes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Building.Infrastructure.Database.Configurations;

internal sealed class ComplexConfiguration : IEntityTypeConfiguration<Complex>
{
	public void Configure(EntityTypeBuilder<Complex> builder)
	{
		builder.HasIndex(item => item.Name);

		builder.HasIndex(item => item.IsCommercial);

		builder.HasIndex(item => item.IsLiving);

		builder.HasIndex(item => item.IsActive);
	}
}
