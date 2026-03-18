using Didox.Application.Abstractions.Database;
using Didox.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Didox.Infrastructure;

/// <summary>
/// Database context for Didox module
/// </summary>
public sealed class DidoxDbContext : DbContext, IDidoxDbContext
{
    public const string SchemaName = "didox";
    
    public DidoxDbContext(DbContextOptions<DidoxDbContext> options) 
        : base(options)
    {
    }
    
    DatabaseFacade IDidoxDbContext.Database => base.Database;
    
    public DbSet<DidoxAccount> Accounts { get; set; }
    public DbSet<DidoxToken> Tokens { get; set; }
    
    DbSet<DidoxAccount> IDidoxDbContext.Accounts => Accounts;
    DbSet<DidoxToken> IDidoxDbContext.Tokens => Tokens;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(SchemaName.ToLowerInvariant());
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DidoxDbContext).Assembly);
        
        base.OnModelCreating(modelBuilder);
    }
}
