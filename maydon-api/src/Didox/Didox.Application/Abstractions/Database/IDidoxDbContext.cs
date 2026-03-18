using Didox.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Didox.Application.Abstractions.Database;

public interface IDidoxDbContext
{
    DatabaseFacade Database { get; }
    DbSet<DidoxAccount> Accounts { get; }
    DbSet<DidoxToken> Tokens { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
