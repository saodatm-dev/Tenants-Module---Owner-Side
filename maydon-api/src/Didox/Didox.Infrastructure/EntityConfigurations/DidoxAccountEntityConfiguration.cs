using Didox.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Didox.Infrastructure.EntityConfigurations;

public class DidoxAccountEntityConfiguration : IEntityTypeConfiguration<DidoxAccount>
{
    public void Configure(EntityTypeBuilder<DidoxAccount> builder)
    {
        builder.HasIndex(x => x.OwnerId);
        builder.HasIndex(x => x.Tin);
        builder.HasIndex(x => x.Pinfl);
        
        builder.Property(x => x.Login).HasMaxLength(250).IsRequired();
        builder.Property(x => x.Password).HasMaxLength(250).IsRequired();
        builder.Property(x=>x.Tin).HasMaxLength(250);
        builder.Property(x => x.Pinfl).HasMaxLength(250);
        
        builder.Property(x =>x.CreatedDate).IsRequired().HasDefaultValueSql("now()");
    }
}
