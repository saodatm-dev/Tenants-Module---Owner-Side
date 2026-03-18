using Building.Application.Core.Abstractions.Data;
using Building.Application.RealEstateTypes.Create;
using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Messaging;
using Core.Application.Abstractions.Services.Minio;
using Core.Application.Resources;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.RealEstateTypes.Update;

internal sealed class UpdateRealEstateTypeCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IExecutionContextProvider executionContextProvider,
	IFileManager fileManager,
	IBuildingDbContext dbContext) : ICommandHandler<UpdateRealEstateTypeCommand, Guid>
{
	public async Task<Result<Guid>> Handle(UpdateRealEstateTypeCommand command, CancellationToken cancellationToken)
	{
		var maybeItem = await dbContext.RealEstateTypes.FindAsync([command.Id], cancellationToken);
		if (maybeItem is null)
			return Result<Guid>.ValidationFailure(sharedViewLocalizer.RealEstateTypeNotFound(nameof(UpdateRealEstateTypeCommand.Id)));

		// TODO : Validation for other fields
		if (await dbContext.RealEstateTypeTranslates.AnyAsync(item =>
			item.Field == Domain.RealEstateTypes.RealEstateTypeField.Name &&
			item.Value == command.Names.First().Value,
			cancellationToken))

			return Result<Guid>.ValidationFailure(sharedViewLocalizer.AlreadyExists(nameof(CreateRealEstateTypeCommand.Names)));

		string? iconUrl = command.IconUrl;
		if (!string.IsNullOrWhiteSpace(command.IconUrl) && maybeItem.IconUrl != iconUrl)
		{
			var iconUrlResult = await fileManager.CopyToPublicAsync(command.IconUrl, $"{executionContextProvider.TenantId}", cancellationToken: cancellationToken);
			if (iconUrlResult.IsFailure)
				return Result<Guid>.ValidationFailure(sharedViewLocalizer.InvalidValue(nameof(CreateRealEstateTypeCommand.IconUrl)));

			iconUrl = iconUrlResult.Value;
		}

		dbContext.RealEstateTypes.Update(
			maybeItem.Update(
				command.TypeName,
				command.Names,
				command.Descriptions,
				iconUrl,
				command.ShowBuildingSuggestion,
				command.ShowFloorSuggestion,
				command.CanHaveUnits,
				command.CanHaveMeters,
				command.CanHaveFloors));

		await dbContext.SaveChangesAsync(cancellationToken);

		return maybeItem.Id;
	}
}
