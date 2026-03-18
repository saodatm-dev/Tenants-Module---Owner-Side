using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Messaging;
using Core.Application.Abstractions.Services.Minio;
using Core.Application.Resources;
using Core.Domain.Results;
using Identity.Application.Core.Abstractions.Data;
using Identity.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Identity.Application.Users.UpdateLogo;

internal sealed class UpdateUserLogoCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IExecutionContextProvider executionContextProvider,
	IIdentityDbContext dbContext,
	IFileManager fileManager) : ICommandHandler<UpdateUserLogoCommand>
{
	public async Task<Result> Handle(UpdateUserLogoCommand command, CancellationToken cancellationToken)
	{
		var maybeItem = await dbContext.Users.FirstOrDefaultAsync(item => item.Id == executionContextProvider.TenantId, cancellationToken);
		if (maybeItem is null)
			return Result.Failure<Guid>(sharedViewLocalizer.UserNotFound(nameof(User)));

		//var objectNameResult = await fileManager.CopyToPublicAsync(command.ObjectName, $"{executionContextProvider.TenantId}", true, cancellationToken: cancellationToken);
		//if (objectNameResult.IsFailure)
		//	return Result.Failure<Guid>(sharedViewLocalizer.LogoNotFound(nameof(UpdateUserLogoCommand.ObjectName)));

		dbContext.Users.Update(maybeItem.UpdateLogo(command.ObjectName));

		await dbContext.SaveChangesAsync(cancellationToken);

		return Result.Success();
	}
}
