using Identity.Domain.IntegrationService;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Infrastructure.Database.Configurations;

internal sealed class IntegrationServiceConfiguration : IEntityTypeConfiguration<IntegrationService>
{
	public void Configure(EntityTypeBuilder<IntegrationService> builder)
	{
		builder.HasKey(item => item.Id);

		builder.HasIndex(item => item.Type);

		builder.Property(item => item.Type)
			.HasConversion<short>();
	}
}
