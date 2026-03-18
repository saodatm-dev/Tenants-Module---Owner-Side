using Building.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Building.Infrastructure;

/// <summary>
/// Design-time factory for creating BuildingDbContext instances for EF Core migrations
/// </summary>
internal class BuildingDbContextFactory : IDesignTimeDbContextFactory<BuildingDbContext>
{
    public BuildingDbContext CreateDbContext(string[] args)
    {
        var basePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "Maydon.Host");
        if (!Directory.Exists(basePath))
        {
            basePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "Maydon.Main.Host");
        }
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

        var optionsBuilder = new DbContextOptionsBuilder<BuildingDbContext>();

        optionsBuilder.UseNpgsql(
            connectionString,
            npgsqlOptions =>
            {
                npgsqlOptions.MigrationsHistoryTable(
                    Domain.AssemblyReference.MigrationHistoryTableName,
                    Domain.AssemblyReference.Instance);
            npgsqlOptions.MigrationsAssembly(typeof(BuildingDbContext).Assembly.FullName);
                npgsqlOptions.UseNetTopologySuite();
            })
            .UseSnakeCaseNamingConvention();

        return new BuildingDbContext(optionsBuilder.Options);
    }
}
