using ContractEntity = Document.Domain.Contracts.Contract;
using Document.Domain.Contracts;
using Document.Domain.ContractTemplates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Document.Application.Abstractions.Data;

/// <summary>
/// Application database context interface for dependency injection
/// </summary>
public interface IDocumentDbContext
{
    DatabaseFacade Database { get; }

    DbSet<ContractTemplate> ContractTemplates { get; }
    DbSet<ContractEntity> Contracts { get; }
    DbSet<ContractAttachment> ContractAttachments { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

