using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Identity.Application.Companies.Remove;
using Identity.Application.Core.Abstractions.Data;
using Microsoft.EntityFrameworkCore;

namespace Identity.Application.CompanyUsers.RemoveBulk;

internal sealed class RemoveCompanyUsersCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IIdentityDbContext dbContext) : ICommandHandler<RemoveCompanyUsersCommand>
{
	public async Task<Result> Handle(RemoveCompanyUsersCommand command, CancellationToken cancellationToken)
	{
		var maybeItems = await dbContext.CompanyUsers
			.Where(item => command.UserIds.Contains(item.UserId))
			.ToListAsync(cancellationToken);

		if (!maybeItems.Any())
			return Result.Failure<Guid>(sharedViewLocalizer.UserNotFound(nameof(RemoveCompanyCommand.Id)));

		dbContext.CompanyUsers.UpdateRange(maybeItems.Select(item => item.Remove()));

		await dbContext.SaveChangesAsync(cancellationToken);

		return Result.Success();
	}
}
