using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Identity.Application.Core.Abstractions.Data;
using Microsoft.EntityFrameworkCore;

namespace Identity.Application.Companies.Remove;

internal sealed class RemoveCompanyCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IExecutionContextProvider executionContextProvider,
	IIdentityDbContext dbContext) : ICommandHandler<RemoveCompanyCommand, Guid>
{
	public async Task<Result<Guid>> Handle(RemoveCompanyCommand command, CancellationToken cancellationToken)
	{
		var maybeItem = await dbContext.Companies.FirstOrDefaultAsync(item => item.Id == command.Id && item.OwnerId == executionContextProvider.UserId, cancellationToken);

		if (maybeItem is null)
			return Result.Failure<Guid>(sharedViewLocalizer.CompanyNotFound(nameof(RemoveCompanyCommand.Id)));

		dbContext.Companies.Update(maybeItem.Remove());

		await dbContext.SaveChangesAsync(cancellationToken);

		return maybeItem.Id;
	}
}
