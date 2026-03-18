using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Messaging;
using Core.Application.Abstractions.Services.Minio;
using Core.Application.Resources;
using Core.Domain.Results;
using Identity.Application.Core.Abstractions.Data;
using Identity.Domain.Companies;
using Microsoft.EntityFrameworkCore;

namespace Identity.Application.Companies.UpdateLogo;

internal sealed class UpdateCompanyLogoCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IExecutionContextProvider executionContextProvider,
	IIdentityDbContext dbContext,
	IFileManager fileManager) : ICommandHandler<UpdateCompanyLogoCommand>
{
	public async Task<Result> Handle(UpdateCompanyLogoCommand command, CancellationToken cancellationToken)
	{
		var maybeItem = await dbContext.Companies.FirstOrDefaultAsync(item => item.Id == executionContextProvider.TenantId, cancellationToken);
		if (maybeItem is null)
			return Result.Failure<Guid>(sharedViewLocalizer.CompanyNotFound(nameof(Company)));

		var objectNameResult = await fileManager.CopyToPublicAsync(command.ObjectName, $"{executionContextProvider.TenantId}", true, cancellationToken: cancellationToken);
		if (objectNameResult.IsFailure)
			return Result.Failure<Guid>(sharedViewLocalizer.LogoNotFound(nameof(UpdateCompanyLogoCommand.ObjectName)));

		dbContext.Companies.Update(maybeItem.UpdateLogo(objectNameResult.Value));

		await dbContext.SaveChangesAsync(cancellationToken);

		return Result.Success();
	}
}
