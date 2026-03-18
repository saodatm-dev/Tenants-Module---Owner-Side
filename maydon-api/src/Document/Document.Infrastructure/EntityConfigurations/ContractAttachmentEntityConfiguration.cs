using Document.Domain.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Document.Infrastructure.EntityConfigurations;

public sealed class ContractAttachmentEntityConfiguration : IEntityTypeConfiguration<ContractAttachment>
{
    public void Configure(EntityTypeBuilder<ContractAttachment> builder)
    {
        builder.ToTable("contract_attachments", DocumentDbContext.SchemaName.ToLowerInvariant());

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.ContractId).IsRequired();

        builder.Property(x => x.FileName)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(x => x.ObjectKey)
            .IsRequired()
            .HasMaxLength(512);

        builder.Property(x => x.ContentType)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(x => x.FileSize).IsRequired();

        builder.Property(x => x.UploadedAt)
            .IsRequired()
            .HasDefaultValueSql("now()");

        builder.Property(x => x.UploadedByUserId).IsRequired();

        builder.HasIndex(x => x.ContractId);
    }
}
