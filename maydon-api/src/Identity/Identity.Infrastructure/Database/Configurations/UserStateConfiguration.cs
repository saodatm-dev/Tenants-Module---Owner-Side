using Identity.Domain.UserStates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Infrastructure.Database.Configurations;

internal sealed class UserStateConfiguration : IEntityTypeConfiguration<UserState>
{
	public void Configure(EntityTypeBuilder<UserState> builder)
	{
		builder.HasKey(item => item.Id);

		builder.HasIndex(item => item.Id).IsUnique();

		builder.HasIndex(item => item.PhoneNumber);

		builder.Property(item => item.PhoneNumber)
			.IsRequired()
			.HasMaxLength(30);
	}
}
