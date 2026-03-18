using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Didox.Infrastructure;

/// <summary>
/// Design-time factory for creating DidoxDbContext instances for EF Core migrations
/// </summary>
public class DidoxDbContextFactory : IDesignTimeDbContextFactory<DidoxDbContext>
{
    public DidoxDbContext CreateDbContext(string[] args)
    {
        var basePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "Maydon.Main.Host");
        if (!Directory.Exists(basePath))
        {
            basePath = Directory.GetCurrentDirectory();
        }

        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString(Core.Infrastructure.Extensions.DatabaseExtensions.DatabaseName)
            ?? "Host=localhost;Database=maydon_db;Username=postgres;Password=postgres";

        var optionsBuilder = new DbContextOptionsBuilder<DidoxDbContext>();

        optionsBuilder.UseNpgsql(
            connectionString,
            npgsqlOptions =>
            {
                npgsqlOptions.MigrationsHistoryTable("__ef_migrations_history", DidoxDbContext.SchemaName);
                npgsqlOptions.MigrationsAssembly(typeof(DidoxDbContext).Assembly.FullName);
            })
            .UseSnakeCaseNamingConvention();

        return new DidoxDbContext(optionsBuilder.Options);
    }
}
