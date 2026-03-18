using System.Text.RegularExpressions;
using Asp.Versioning;
using Maydon.Host.Abstractions;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Maydon.Host.Extensions;

internal static partial class EndpointExtensions
{
	[GeneratedRegex(@"(?<!^)(?=[A-Z])")]
	private static partial Regex KebabCaseRegex();

	private static string ToKebabCase(string name) =>
		KebabCaseRegex().Replace(name, "-").ToLowerInvariant();

	extension(IServiceCollection services)
	{
		internal IServiceCollection AddEndpoints()
		{
			var serviceDescriptors = typeof(DependencyInjection).Assembly
				.DefinedTypes
				.Where(type => type is { IsAbstract: false, IsInterface: false } &&
							   type.IsAssignableTo(typeof(IEndpoint)))
				.Select(type => ServiceDescriptor.Transient(typeof(IEndpoint), type))
				.ToArray();

			services.TryAddEnumerable(serviceDescriptors);

			return services;
		}
	}

	extension(WebApplication app)
	{
		internal WebApplication MapEndpoints(RouteGroupBuilder? routeGroupBuilder = null)
		{
			var endpoints = app.Services.GetRequiredService<IEnumerable<IEndpoint>>();

			IEndpointRouteBuilder builder = routeGroupBuilder is null ? app : routeGroupBuilder;

			var apiVersionSet = app.NewApiVersionSet()
			.HasApiVersion(new ApiVersion(1))
			.HasApiVersion(new ApiVersion(2))
			.ReportApiVersions()
			.Build();

			foreach (IEndpoint endpoint in endpoints)
			{
				var routeName = endpoint.Route ?? ToKebabCase(endpoint.GetType().Name);
				var endPointRouteGroupBuilder = builder.MapGroup("/api/v{version:apiVersion}/" + routeName)
					.WithTags(endpoint.GetType().Name)
					.WithApiVersionSet(apiVersionSet)
					.MapToApiVersion(1)
					.RequireRateLimiting(RateLimiterExtensions.RateLimiterPolicyName)
					.RequireAuthorization()
					.WithGroupName(endpoint.GroupName);
				//.DisableAntiforgery();

				endpoint.MapEndpoint(endPointRouteGroupBuilder);
			}

			return app;
		}
	}

	extension(RouteHandlerBuilder app)
	{
		internal RouteHandlerBuilder HasPermission(string permission) => app.RequireAuthorization(permission);
	}
}
