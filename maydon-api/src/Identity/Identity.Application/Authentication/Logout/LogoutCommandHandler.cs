using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Identity.Application.Core.Abstractions.Data;

namespace Identity.Application.Authentication.Logout;

internal sealed class LogoutCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IExecutionContextProvider executionContextProvider,
	IIdentityDbContext dbContext) : ICommandHandler<LogoutCommand>
{
	public async Task<Result> Handle(LogoutCommand command, CancellationToken cancellationToken)
	{
		var session = await dbContext.Sessions.FindAsync(new object?[] { executionContextProvider.SessionId }, cancellationToken);
		if (session is null)
			return Result.Failure<AuthenticationResponse>(sharedViewLocalizer.InvalidValue(nameof(LogoutCommand)));

		dbContext.Sessions.Remove(session);

		await dbContext.SaveChangesAsync(cancellationToken);

		return Result.Success();
	}
}
