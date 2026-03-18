using System.Reflection;

namespace Core.Application.Abstractions.Database;

/// <summary>
/// Describes a module's database migration requirements.
/// </summary>
public sealed record ModuleMigrationDescriptor
{
    public required string ModuleName { get; init; }
    public required string SchemaName { get; init; }
    public required Type DbContextType { get; init; }
    public required Assembly MigrationsAssembly { get; init; }
    public bool HasSqlScripts { get; init; }
    public int Order { get; init; }
}

/// <summary>
/// Registry that collects module migration descriptors from each module's DI setup.
/// </summary>
public interface IModuleMigrationRegistry
{
    IReadOnlyList<ModuleMigrationDescriptor> GetDescriptors();
    void Register(ModuleMigrationDescriptor descriptor);
}
