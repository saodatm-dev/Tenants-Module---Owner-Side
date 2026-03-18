using Identity.Domain.Otps;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Infrastructure.Database.Configurations;

internal sealed class OtpConfiguration : IEntityTypeConfiguration<Otp>
{
	public void Configure(EntityTypeBuilder<Otp> builder)
	{
		builder.HasKey(item => item.Id);

		builder.HasIndex(item => item.PhoneNumber);
	}
}
