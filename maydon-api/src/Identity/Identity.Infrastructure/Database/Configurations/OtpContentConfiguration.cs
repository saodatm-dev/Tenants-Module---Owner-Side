using Identity.Domain.OtpContents;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Infrastructure.Database.Configurations;

internal sealed class OtpContentConfiguration : IEntityTypeConfiguration<OtpContent>
{
	public void Configure(EntityTypeBuilder<OtpContent> builder)
	{
		builder.HasKey(item => item.Id);

		builder.HasIndex(item => item.OtpType);

		builder.Property(item => item.OtpType)
			.HasConversion<int>();
	}
}
