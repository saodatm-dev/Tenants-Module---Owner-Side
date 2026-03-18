using Document.Domain.ContractTemplates;
using Document.Domain.ContractTemplates.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Document.Infrastructure.EntityConfigurations;

public sealed class ContractTemplateEntityConfiguration : IEntityTypeConfiguration<ContractTemplate>
{
    public void Configure(EntityTypeBuilder<ContractTemplate> builder)
    {
        builder.ToTable("contract_templates", DocumentDbContext.SchemaName.ToLowerInvariant());

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.TenantId);

        builder.Property(x => x.CreatedByUserId)
            .IsRequired();

        builder.Property(x => x.Scope)
            .HasConversion(
                v => v.ToString(),
                v => Enum.Parse<ContractTemplateScope>(v))
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.Category)
            .HasConversion(
                v => v.ToString(),
                v => Enum.Parse<ContractTemplateCategory>(v))
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(x => x.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(x => x.Code)
            .IsUnique();

        builder.Property(x => x.Name)
            .IsRequired()
            .HasColumnType("jsonb");

        builder.Property(x => x.Description)
            .HasColumnType("jsonb");

        builder.Property(x => x.Page)
            .IsRequired()
            .HasColumnType("jsonb");

        builder.Property(x => x.Theme)
            .IsRequired()
            .HasColumnType("jsonb");

        builder.Property(x => x.Header)
            .HasColumnType("jsonb");

        builder.Property(x => x.Footer)
            .HasColumnType("jsonb");

        builder.Property(x => x.Bodies)
            .IsRequired()
            .HasColumnType("jsonb");

        builder.Property(x => x.ManualFields)
            .HasColumnType("jsonb");

        builder.Property(x => x.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(x => x.CurrentVersion)
            .IsRequired()
            .HasDefaultValue(1);

        builder.Property(x => x.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("now()");

        builder.Property(x => x.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("now()");

        // Indexes
        builder.HasIndex(x => x.TenantId);
        builder.HasIndex(x => x.Scope);
        builder.HasIndex(x => x.Category);
        builder.HasIndex(x => x.IsActive);
        builder.HasIndex(x => x.CreatedByUserId);
    }
}
