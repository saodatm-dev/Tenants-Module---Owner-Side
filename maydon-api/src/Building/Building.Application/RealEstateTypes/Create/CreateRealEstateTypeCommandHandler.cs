using Building.Application.Core.Abstractions.Data;
using Building.Domain.RealEstateTypes;
using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Messaging;
using Core.Application.Abstractions.Services.Minio;
using Core.Application.Resources;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.RealEstateTypes.Create;

internal sealed class CreateRealEstateTypeCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IExecutionContextProvider executionContextProvider,
	IFileManager fileManager,
	IBuildingDbContext dbContext) : ICommandHandler<CreateRealEstateTypeCommand, Guid>
{
	public async Task<Result<Guid>> Handle(CreateRealEstateTypeCommand command, CancellationToken cancellationToken)
	{
		// TODO : Validation for other fields
		if (await dbContext.RealEstateTypeTranslates.AnyAsync(item =>
			item.Field == Domain.RealEstateTypes.RealEstateTypeField.Name &&
			item.Value == command.Names.First().Value,
			cancellationToken))

			return Result<Guid>.ValidationFailure(sharedViewLocalizer.AlreadyExists(nameof(CreateRealEstateTypeCommand.Names)));

		//string? iconUrl = command.IconUrl;
		//if (!string.IsNullOrWhiteSpace(command.IconUrl))
		//{
		//	var iconUrlResult = await fileManager.CopyToPublicAsync(command.IconUrl, $"{executionContextProvider.TenantId}", cancellationToken: cancellationToken);
		//	if (iconUrlResult.IsFailure)
		//		return Result<Guid>.ValidationFailure(sharedViewLocalizer.InvalidValue(nameof(CreateRealEstateTypeCommand.IconUrl)));

		//	iconUrl = iconUrlResult.Value;
		//}

		var item = new RealEstateType(
			command.TypeName,
			command.Names,
			command.Descriptions,
			command.IconUrl,
			command.ShowBuildingSuggestion,
			command.ShowFloorSuggestion,
			command.CanHaveUnits,
			command.CanHaveMeters,
			command.CanHaveFloors);

		await dbContext.RealEstateTypes.AddAsync(item, cancellationToken);

		await dbContext.SaveChangesAsync(cancellationToken);

		return item.Id;
	}
}
