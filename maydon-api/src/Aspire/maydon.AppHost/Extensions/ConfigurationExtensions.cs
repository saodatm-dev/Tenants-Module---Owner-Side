using Microsoft.Extensions.Configuration;

namespace maydon.AppHost.Extensions;

internal static class ConfigurationExtensions
{
	extension(IConfiguration config)
	{
		public T GetRequired<T>(
			string key,
			string? envVarName = null) where T : IConvertible
		{
			try
			{
				// Check environment variable first
				if (!string.IsNullOrEmpty(envVarName))
				{
					var envValue = Environment.GetEnvironmentVariable(envVarName);
					if (!string.IsNullOrEmpty(envValue))
						return (T)Convert.ChangeType(envValue, typeof(T));
				}

				// Fall back to configuration
				var value = config[key];
				if (string.IsNullOrEmpty(value))
					throw new InvalidOperationException($"Configuration '{key}' is required");

				return (T)Convert.ChangeType(value, typeof(T));
			}
			catch
			{
				return default(T);
			}
		}

		public T? GetOrDefault<T>(
			string key,
			string? envVarName = null,
			T? defaultValue = default(T)) where T : IConvertible
		{
			try
			{
				if (!string.IsNullOrEmpty(envVarName))
				{
					var envValue = Environment.GetEnvironmentVariable(envVarName);
					if (!string.IsNullOrEmpty(envValue))
						return (T)Convert.ChangeType(envValue, typeof(T));
				}

				return (T)Convert.ChangeType(config[key], typeof(T));
			}
			catch
			{
				return defaultValue;
			}
		}
	}
}
