using System.Reflection;
using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Core.Infrastructure.Database;

public static class ModelBuilderExtensions
{
	extension(ModelBuilder modelBuilder)
	{
		public IEnumerable<Type> GetEntityTypes(Assembly assembly)
		{
			return assembly.DefinedTypes
				.Where(item => !item.IsAbstract &&
					(item.BaseType == typeof(Entity) || item.BaseType?.BaseType == typeof(Entity)))
				.Select(item => item.AsType());
		}
	}

	public static MethodInfo SetGlobalQueryMethod<T>() where T : DbContext => typeof(T)
		.GetMethods(BindingFlags.Public | BindingFlags.Instance)
		.Single(t => t.IsGenericMethod && t.Name == "SetGlobalQuery");
}
