using Document.Domain.Contracts;
using Document.Domain.Contracts.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Document.Infrastructure.EntityConfigurations;

public sealed class ContractSigningEventEntityConfiguration : IEntityTypeConfiguration<ContractSigningEvent>
{
    public void Configure(EntityTypeBuilder<ContractSigningEvent> builder)
    {
        builder.ToTable("contract_signing_events", DocumentDbContext.SchemaName.ToLowerInvariant());

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.ContractId).IsRequired();

        builder.Property(x => x.Party)
            .HasConversion(
                v => v.ToString(),
                v => Enum.Parse<SigningParty>(v))
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.Action)
            .HasConversion(
                v => v.ToString(),
                v => Enum.Parse<SigningAction>(v))
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.OccurredAt).IsRequired();

        builder.Property(x => x.ExternalSignatureId).HasMaxLength(256);

        builder.HasIndex(x => x.ContractId);
    }
}
