using Document.Domain.Contracts;
using Document.Domain.Contracts.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Document.Infrastructure.EntityConfigurations;

public sealed class ContractFinancialItemEntityConfiguration : IEntityTypeConfiguration<ContractFinancialItem>
{
    public void Configure(EntityTypeBuilder<ContractFinancialItem> builder)
    {
        builder.ToTable("contract_financial_items", DocumentDbContext.SchemaName.ToLowerInvariant());

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.ContractId).IsRequired();

        builder.Property(x => x.Type)
            .HasConversion(
                v => v.ToString(),
                v => Enum.Parse<FinancialItemType>(v))
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(x => x.Amount).IsRequired();

        builder.Property(x => x.Frequency)
            .HasConversion(
                v => v.ToString(),
                v => Enum.Parse<FinancialFrequency>(v))
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.SortOrder).IsRequired();

        builder.HasIndex(x => x.ContractId);
    }
}
