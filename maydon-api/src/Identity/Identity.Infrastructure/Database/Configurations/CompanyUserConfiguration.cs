using Identity.Domain.CompanyUsers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Infrastructure.Database.Configurations;

internal sealed class CompanyUserConfiguration : IEntityTypeConfiguration<CompanyUser>
{
	public void Configure(EntityTypeBuilder<CompanyUser> builder)
	{
		builder.HasKey(item => item.Id);

		builder.HasIndex(item => item.CompanyId);

		builder.HasIndex(item => item.UserId);
	}
}
