using Core.Application.Abstractions.Database;

namespace Core.Infrastructure.Database;

/// <summary>
/// In-memory singleton registry that collects module migration descriptors.
/// Each module registers its descriptor during DI setup.
/// </summary>
public sealed class ModuleMigrationRegistry : IModuleMigrationRegistry
{
    private readonly List<ModuleMigrationDescriptor> _descriptors = [];

    public IReadOnlyList<ModuleMigrationDescriptor> GetDescriptors() =>
        _descriptors.OrderBy(d => d.Order).ToList();

    public void Register(ModuleMigrationDescriptor descriptor) =>
        _descriptors.Add(descriptor);
}
