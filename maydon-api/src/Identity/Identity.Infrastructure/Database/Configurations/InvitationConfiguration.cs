using Identity.Domain.Invitations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Infrastructure.Database.Configurations;

internal sealed class InvitationConfiguration : IEntityTypeConfiguration<Invitation>
{
	public void Configure(EntityTypeBuilder<Invitation> builder)
	{
		builder.HasKey(item => item.Id);

		builder.HasIndex(item => item.Status);

		builder.Property(item => item.Status)
			.HasConversion<short>();
	}
}
