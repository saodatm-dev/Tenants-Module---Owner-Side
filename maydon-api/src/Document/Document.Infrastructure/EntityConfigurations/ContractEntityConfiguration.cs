using Document.Domain.Contracts.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Document.Infrastructure.EntityConfigurations;

public sealed class ContractEntityConfiguration : IEntityTypeConfiguration<Domain.Contracts.Contract>
{
    public void Configure(EntityTypeBuilder<Domain.Contracts.Contract> builder)
    {
        builder.ToTable("contracts", DocumentDbContext.SchemaName.ToLowerInvariant());

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        // ─── Core properties ───

        builder.Property(x => x.TenantId).IsRequired();

        builder.Property(x => x.ContractNumber).HasMaxLength(50);

        builder.Property(x => x.TemplateId).IsRequired();

        builder.Property(x => x.Language)
            .IsRequired()
            .HasMaxLength(5);

        builder.Property(x => x.Body)
            .IsRequired()
            .HasColumnType("jsonb");

        // ─── References ───

        builder.Property(x => x.LeaseId).IsRequired();
        builder.Property(x => x.RealEstateId).IsRequired();
        builder.Property(x => x.OwnerCompanyId).IsRequired();
        builder.Property(x => x.ClientCompanyId); // nullable

        // ─── Party identification ───

        builder.Property(x => x.OwnerInn).HasMaxLength(14);
        builder.Property(x => x.OwnerPinfl).HasMaxLength(14);
        builder.Property(x => x.ClientInn).HasMaxLength(14);
        builder.Property(x => x.ClientPinfl).HasMaxLength(14);

        // ─── Lease terms ───

        builder.Property(x => x.MonthlyAmount).IsRequired();
        builder.Property(x => x.LeaseStartDate).IsRequired();

        // ─── Status & dates ───

        builder.Property(x => x.Status)
            .HasConversion(
                v => v.ToString(),
                v => Enum.Parse<ContractStatus>(v))
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.ContractDate)
            .IsRequired()
            .HasDefaultValueSql("now()");

        builder.Property(x => x.CreatedByUserId).IsRequired();

        // ─── Versioning ───

        builder.Property(x => x.CurrentVersion)
            .IsRequired()
            .HasDefaultValue(1);

        // ─── Rejection & deadline ───

        builder.Property(x => x.RejectionReason).HasMaxLength(2000);
        builder.Property(x => x.SignatureDeadline);

        // ─── Soft delete ───

        builder.Property(x => x.IsDeleted).HasDefaultValue(false);

        // ─── Self-referencing parent/child ───

        builder.HasOne(x => x.Parent)
            .WithMany(x => x.ChildContracts)
            .HasForeignKey(x => x.ParentId)
            .OnDelete(DeleteBehavior.Restrict);

        // ─── Owned: Integration states ───

        builder.OwnsMany(x => x.IntegrationStates, a =>
        {
            a.ToTable("contract_provider_states", DocumentDbContext.SchemaName.ToLowerInvariant());
            a.WithOwner().HasForeignKey("ContractId");
            a.HasKey("ContractId", "ProviderName");
            a.Property(p => p.ProviderName).HasMaxLength(50).IsRequired();
            a.Property(p => p.ExternalId).HasMaxLength(256);
            a.Property(p => p.SyncStatus)
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();
            a.Property(p => p.LastUpdated).IsRequired();
            a.Property(p => p.ErrorMessage).HasMaxLength(500);
        });

        // ─── Has-many: Financial items ───

        builder.HasMany(x => x.FinancialItems)
            .WithOne()
            .HasForeignKey(x => x.ContractId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.FinancialItems)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        // ─── Has-many: Signing events ───

        builder.HasMany(x => x.SigningEvents)
            .WithOne()
            .HasForeignKey(x => x.ContractId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.SigningEvents)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        // ─── Has-many: Attachments ───

        builder.HasMany(x => x.Attachments)
            .WithOne()
            .HasForeignKey(x => x.ContractId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.Attachments)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        // ─── Indexes ───

        builder.HasIndex(x => x.TenantId);
        builder.HasIndex(x => x.LeaseId);
        builder.HasIndex(x => x.TemplateId);
        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => x.ParentId);
        builder.HasIndex(x => x.CreatedByUserId);

        builder.HasIndex(x => new { x.TenantId, x.ContractNumber })
            .IsUnique()
            .HasFilter("contract_number IS NOT NULL");
    }
}
