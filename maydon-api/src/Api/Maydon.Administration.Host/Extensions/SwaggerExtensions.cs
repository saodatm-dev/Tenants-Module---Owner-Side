using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;

namespace Maydon.Administration.Host.Extensions;

internal static class SwaggerExtensions
{
	extension(IServiceCollection services)
	{
		internal IServiceCollection AddSwaggerInternal()
		{
			services.AddEndpointsApiExplorer();
			services.AddSwaggerGen(options =>
			{
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

				options.SwaggerDoc("documents", new OpenApiInfo
				{
					Version = "v1",
					Title = "Document api",
					Description = "Document & Contract api endpoints"
				});

				options.SwaggerDoc("didox", new OpenApiInfo
				{
					Version = "v1",
					Title = "Didox api",
					Description = "Didox integration api endpoints"
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
				options.SwaggerEndpoint("/swagger/documents/swagger.json", "Document api");
				options.SwaggerEndpoint("/swagger/didox/swagger.json", "Didox api");

			});

			return app;
		}
	}
}
