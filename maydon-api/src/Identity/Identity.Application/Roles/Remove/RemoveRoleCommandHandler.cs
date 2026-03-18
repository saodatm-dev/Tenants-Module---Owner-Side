using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Identity.Application.Core.Abstractions.Data;

using Microsoft.EntityFrameworkCore;

namespace Identity.Application.Roles.Remove;

internal sealed class RemoveRoleCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IExecutionContextProvider executionContextProvider,
	IIdentityDbContext dbContext) : ICommandHandler<RemoveRoleCommand>
{
	public async Task<Result> Handle(RemoveRoleCommand command, CancellationToken cancellationToken)
	{
		var maybeItem = await dbContext.Roles
			.FirstOrDefaultAsync(item => item.Id == command.Id && item.TenantId == executionContextProvider.TenantId, cancellationToken);

		if (maybeItem is null)
			return Result.Failure<Guid>(sharedViewLocalizer.RoleNotFound(nameof(RemoveRoleCommand.Id)));

		dbContext.Roles.Remove(maybeItem);

		await dbContext.SaveChangesAsync(cancellationToken);

		return Result.Success();
	}
}
