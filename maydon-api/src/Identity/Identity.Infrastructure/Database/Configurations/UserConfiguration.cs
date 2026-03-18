using Identity.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Infrastructure.Database.Configurations;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
	public void Configure(EntityTypeBuilder<User> builder)
	{
		builder.HasKey(item => item.Id);

		builder.HasIndex(item => item.Id).IsUnique();

		builder.HasIndex(item => item.PhoneNumber);

		builder.Property(item => item.PhoneNumber)
			.HasMaxLength(20);

		builder.Property(item => item.Password);

		builder.Property(item => item.Salt);

		builder.Property(item => item.FirstName)
			.HasMaxLength(100);

		builder.Property(item => item.LastName)
			.HasMaxLength(100);

		builder.Property(item => item.MiddleName)
			.HasMaxLength(100);

		builder.Property(item => item.Tin)
			.HasMaxLength(15);

		builder.Property(item => item.Pinfl)
			.HasMaxLength(14);

		builder.Property(item => item.SerialNumber)
			.HasMaxLength(100);

		builder.Property(item => item.PassportNumber)
			.HasMaxLength(20);

		builder.Property(item => item.Address)
			.HasMaxLength(500);

		builder.Property(item => item.ObjectName)
			.HasMaxLength(200);

		builder.HasIndex(item => item.IsActive);

		builder.Property(item => item.RegisterType)
			.HasConversion<int>();
	}
}
