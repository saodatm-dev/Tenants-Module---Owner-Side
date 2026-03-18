using Core.Infrastructure.Converters;
using Document.Application.Abstractions.Data;
using ContractEntity = Document.Domain.Contracts.Contract;
using Document.Domain.Contracts;
using Document.Domain.ContractTemplates;
using Document.Infrastructure.EntityConfigurations;
using Document.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Document.Infrastructure;

/// <summary>
/// Application database context for Entity Framework Core with versioning support
/// </summary>
public sealed class DocumentDbContext(DbContextOptions<DocumentDbContext> options) : DbContext(options), IDocumentDbContext
{
    public const string SchemaName = "documents";


    DatabaseFacade IDocumentDbContext.Database => base.Database;

    public DbSet<ContractTemplate> ContractTemplates => Set<ContractTemplate>();
    public DbSet<ContractEntity> Contracts => Set<ContractEntity>();
    public DbSet<ContractAttachment> ContractAttachments => Set<ContractAttachment>();

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<Core.Domain.ValueObjects.Money>()
            .HaveConversion<MoneyToDecimalConverter>()
            .HaveColumnType("numeric(18,2)");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(SchemaName);

        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new ContractTemplateEntityConfiguration());
        modelBuilder.ApplyConfiguration(new ContractEntityConfiguration());
        modelBuilder.ApplyConfiguration(new ContractFinancialItemEntityConfiguration());
        modelBuilder.ApplyConfiguration(new ContractSigningEventEntityConfiguration());
        modelBuilder.ApplyConfiguration(new ContractAttachmentEntityConfiguration());

        modelBuilder.ApplySoftDeleteQueryFilter();
    }
}

