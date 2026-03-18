using Didox.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Didox.Infrastructure.EntityConfigurations;

/// <summary>
/// Entity configuration for DidoxToken
/// </summary>
public class DidoxTokenEntityConfiguration : IEntityTypeConfiguration<DidoxToken>
{
    public void Configure(EntityTypeBuilder<DidoxToken> builder)
    {
        builder.ToTable("didox_tokens", DidoxDbContext.SchemaName.ToLowerInvariant());
        
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Id)
            .ValueGeneratedNever();
        
        builder.Property(x => x.Token)
            .HasMaxLength(250)
            .HasColumnType("varchar(250)");
        
        builder.Property(x => x.OwnerId)
            .IsRequired();
        
        builder.Property(x => x.CreatedDate)
            .IsRequired()
            .HasDefaultValueSql("now()");
        
        builder.Property(x => x.ExpiresIn)
            .IsRequired();
        
        // Indexes for common queries
        builder.HasIndex(x => x.OwnerId)
            .HasDatabaseName("ix_didox_tokens_owner_id");
    }
}
