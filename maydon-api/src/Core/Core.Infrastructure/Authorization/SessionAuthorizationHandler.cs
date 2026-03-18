using Core.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Infrastructure.Authorization;

internal sealed class SessionAuthorizationHandler(IServiceScopeFactory serviceScopeFactory) : IAuthorizationHandler
{
	public async Task HandleAsync(AuthorizationHandlerContext context)
	{
		if (context.User is { Identity.IsAuthenticated: false })
		{
			context.Fail();

			return;
		}

		using IServiceScope scope = serviceScopeFactory.CreateScope();
		var sessionProvider = scope.ServiceProvider.GetRequiredService<ISessionProvider>();

		// is session actual 
		if (!await sessionProvider.IsActual(context.User.GetSessionId(), context.User.GetAccountId()))
		{
			context.Fail();
			return;
		}
	}
}
