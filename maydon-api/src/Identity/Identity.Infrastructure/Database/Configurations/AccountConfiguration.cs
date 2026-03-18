using Identity.Domain.Accounts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Infrastructure.Database.Configurations;

internal sealed class AccountConfiguration : IEntityTypeConfiguration<Account>
{
	public void Configure(EntityTypeBuilder<Account> builder)
	{
		builder.HasKey(item => item.Id);

		builder.HasIndex(item => item.Id).IsUnique();

		builder.Property(item => item.Type)
			.HasConversion<int>()
			.IsRequired();
	}
}
