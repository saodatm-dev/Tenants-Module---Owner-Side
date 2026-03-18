using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Maydon.Host.Extensions;

internal static class SwaggerExtensions
{
	extension(IServiceCollection services)
	{
		internal IServiceCollection AddSwaggerInternal()
		{
			//services.ConfigureHttpJsonOptions(options =>
			//{
			//	options.SerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
			//});
			services.AddEndpointsApiExplorer();
			services.AddSwaggerGen(options =>
			{
				options.SchemaFilter<EnumSchemaFilter>();
				options.SwaggerDoc(Identity.Domain.AssemblyReference.Instance, new OpenApiInfo
				{
					Version = "v1",
					Title = "Identity api",
					Description = "Identity api endpoints"
				});

				options.SwaggerDoc(Building.Domain.AssemblyReference.Instance, new OpenApiInfo
				{
					Version = "v1",
					Title = "Building api",
					Description = "Building api endpoints"
				});

				options.SwaggerDoc(Common.Domain.AssemblyReference.Instance, new OpenApiInfo
				{
					Version = "v1",
					Title = "Common api",
					Description = "Common api endpoints"
				});

				options.SwaggerDoc("didox", new OpenApiInfo
				{
					Version = "v1",
					Title = "Didox api",
					Description = "Didox integration endpoints"
				});

				options.SwaggerDoc("documents", new OpenApiInfo
				{
					Version = "v1",
					Title = "Documents api",
					Description = "Document management endpoints"
				});

				options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme,
					new OpenApiSecurityScheme
					{
						Type = SecuritySchemeType.Http,
						Scheme = JwtBearerDefaults.AuthenticationScheme,
						In = ParameterLocation.Header,
						BearerFormat = "JWT",
						Name = "JWT Authentication",
						Description = "Enter your JWT token in this field",
					});
				options.AddSecurityRequirement(new OpenApiSecurityRequirement
				{
					[new OpenApiSecurityScheme
					{
						Reference = new OpenApiReference
						{
							Id = JwtBearerDefaults.AuthenticationScheme,
							Type = ReferenceType.SecurityScheme
						}
					}] = Array.Empty<string>()

					//[new OpenApiSecuritySchemeReference(JwtBearerDefaults.AuthenticationScheme, document)] = []
				});
			});

			return services;
		}
	}

	extension(WebApplication app)
	{
		internal WebApplication UseSwaggerInternal()
		{
			app.UseSwagger();
			app.UseSwaggerUI(options =>
			{
				options.SwaggerEndpoint($"/swagger/{Identity.Domain.AssemblyReference.Instance}/swagger.json", "Identity api");
				options.SwaggerEndpoint($"/swagger/{Building.Domain.AssemblyReference.Instance}/swagger.json", "Building api");
				options.SwaggerEndpoint($"/swagger/{Common.Domain.AssemblyReference.Instance}/swagger.json", "Common api");
				options.SwaggerEndpoint("/swagger/didox/swagger.json", "Didox api");
				options.SwaggerEndpoint("/swagger/documents/swagger.json", "Documents api");
			});
			return app;
		}
	}
}
internal sealed class EnumSchemaFilter : ISchemaFilter
{
	public void Apply(OpenApiSchema schema, SchemaFilterContext context)
	{
		if (!context.Type.IsEnum)
			return;

		var enumStringNames = Enum.GetNames(context.Type);
		IEnumerable<int> enumStringValues;
		try
		{
			enumStringValues = Enum.GetValues(context.Type).Cast<int>();
		}
		catch
		{
			enumStringValues = Enum.GetValues(context.Type).Cast<int>().Select(i => Convert.ToInt32(i));
		}
		var enumStringKeyValuePairs = enumStringNames.Zip(enumStringValues, (name, value) => $"{value} = {name}");

		var enumStringNamesAsOpenApiArray = new OpenApiArray();

		enumStringNamesAsOpenApiArray.AddRange(enumStringNames.Select(name => new OpenApiString(name)).ToArray());

		schema.Description = string.Join("\n", enumStringKeyValuePairs);
		schema.Extensions.Add("x-enum-varnames", enumStringNamesAsOpenApiArray);
	}
}
