using Core.Application.Resources;
using Core.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Infrastructure.Authorization;

internal sealed class PermissionAuthorizationHandler(IServiceScopeFactory serviceScopeFactory) : AuthorizationHandler<PermissionRequirement>
{
	protected override async Task HandleRequirementAsync(
		AuthorizationHandlerContext context,
		PermissionRequirement requirement)
	{
		if (context.User is { Identity.IsAuthenticated: false })
		{
			context.Fail();

			return;
		}

		using IServiceScope scope = serviceScopeFactory.CreateScope();

		var permissionProvider = scope.ServiceProvider.GetRequiredService<PermissionProvider>();

		var roleId = context.User.GetRoleId();

		var permissions = await permissionProvider.GetByRoleIdAsync(roleId);

		//if (permissions.Contains(requirement.Permission))
		//{
		context.Succeed(requirement);

		return;
		//}

		var httpContext = context.Resource as DefaultHttpContext;
		if (httpContext != null && !httpContext.Response.HasStarted)
		{
			var sharedViewLocalizer = scope.ServiceProvider.GetRequiredService<ISharedViewLocalizer>();
			httpContext.Response.StatusCode = 403;

			var accessDeniedError = sharedViewLocalizer.NoAccess("user");
			var problemDetails = new ProblemDetails
			{
				Status = StatusCodes.Status403Forbidden,
				Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.3",
				Title = accessDeniedError.Code,
				Detail = accessDeniedError.Description
			};
			await httpContext.Response.WriteAsJsonAsync(problemDetails);

			return;
		}

		context.Fail(new AuthorizationFailureReason(this, "No access"));

		return;
	}
}

