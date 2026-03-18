using System.Reflection;

namespace Core.Domain;

public static class AssemblyReference
{
	public const string Instance = "core";

	public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}
