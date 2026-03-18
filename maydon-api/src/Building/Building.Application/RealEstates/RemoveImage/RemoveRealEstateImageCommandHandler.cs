using Building.Application.Core.Abstractions.Data;
using Building.Domain.RealEstateImages;
using Core.Application.Abstractions.Messaging;
using Core.Application.Abstractions.Services.Minio;
using Core.Application.Resources;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.RealEstates.RemoveImage;

public sealed class RemoveRealEstateImageCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IBuildingDbContext dbContext,
	IFileManager fileManager) : ICommandHandler<RemoveRealEstateImageCommand>
{
	public async Task<Result> Handle(RemoveRealEstateImageCommand request, CancellationToken cancellationToken)
	{
		var image = await dbContext.RealEstateImages
			.Include(item => item.RealEstate)
			.IsUpdatable()
			.FirstOrDefaultAsync(item => item.Id == request.Id, cancellationToken);

		if (image is null)
			return Result.Failure<Guid>(sharedViewLocalizer.RealEstateNotFound(nameof(RemoveRealEstateImageCommand.Id)));

		var deleteResult = await fileManager.DeleteFileAsync(image.ObjectName, cancellationToken);
		if (deleteResult.IsFailure)
			return Result.Failure(deleteResult.Error);

		dbContext.RealEstateImages.Remove(image);

		await dbContext.SaveChangesAsync(cancellationToken);

		return Result.Success();
	}
}
