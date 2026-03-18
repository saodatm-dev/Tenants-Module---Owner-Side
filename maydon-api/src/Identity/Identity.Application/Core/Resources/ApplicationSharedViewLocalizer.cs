using System.Reflection;
using Core.Application.Resources;
using Microsoft.Extensions.Localization;

namespace Identity.Application.Core.Resources;

internal sealed class ApplicationSharedViewLocalizer : ISharedViewLocalizer
{
	public ApplicationSharedViewLocalizer(IStringLocalizerFactory factory)
	{
		var assemblyName = new AssemblyName(typeof(SharedResource).Assembly.FullName);
		Localizer = factory.Create("SharedResource", assemblyName.Name);
	}

	public LocalizedString this[string key] => Localizer[key];
	public LocalizedString GetLocalizedString(string key) => Localizer[key];
	public IStringLocalizer Localizer { get; }
}
