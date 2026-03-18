using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Identity.Application.Companies.Remove;
using Identity.Application.Core.Abstractions.Data;
using Microsoft.EntityFrameworkCore;

namespace Identity.Application.CompanyUsers.Remove;

internal sealed class RemoveCompanyUserCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IIdentityDbContext dbContext) : ICommandHandler<RemoveCompanyUserCommand, Guid>
{
	public async Task<Result<Guid>> Handle(RemoveCompanyUserCommand command, CancellationToken cancellationToken)
	{
		var maybeItem = await dbContext.CompanyUsers
			.FirstOrDefaultAsync(item => item.UserId == command.UserId, cancellationToken);

		if (maybeItem is null)
			return Result.Failure<Guid>(sharedViewLocalizer.UserNotFound(nameof(RemoveCompanyCommand.Id)));

		dbContext.CompanyUsers.Update(maybeItem.Remove());

		await dbContext.SaveChangesAsync(cancellationToken);

		return maybeItem.Id;
	}
}
