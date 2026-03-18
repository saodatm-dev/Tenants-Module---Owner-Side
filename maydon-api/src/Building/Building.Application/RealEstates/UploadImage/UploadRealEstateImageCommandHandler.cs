using Building.Application.Core.Abstractions.Data;
using Building.Domain.RealEstateImages;
using Building.Domain.RealEstates;
using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Messaging;
using Core.Application.Abstractions.Services.Minio;
using Core.Application.Resources;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.RealEstates.UploadImage;

internal sealed class UploadRealEstateImageCommandHandler(
    ISharedViewLocalizer sharedViewLocalizer,
    IExecutionContextProvider executionContextProvider,
    IBuildingDbContext dbContext,
    IFileManager fileManager) : ICommandHandler<UploadRealEstateImageCommand>
{
    public async Task<Result> Handle(UploadRealEstateImageCommand request, CancellationToken cancellationToken)
    {
        if (!await dbContext.RealEstates
                .AsNoTracking()
                .IsUpdatable(executionContextProvider.TenantId)
                .AnyAsync(item => item.Id == request.RealEstateId, cancellationToken))
            return Result.Failure(sharedViewLocalizer.RealEstateNotFound(nameof(UploadRealEstateImageCommand.RealEstateId)));

        //var objectNameTasks = request.Images
        //		.Select(imageKey =>
        //			fileManager.CopyToPublicAsync(
        //				imageKey,
        //				$"{executionContextProvider.TenantId}",
        //				true,
        //				cancellationToken));

        //var result = await Task.WhenAll(objectNameTasks);

        if (request.Images.Any())
            await dbContext.RealEstateImages.AddRangeAsync(
                request.Images//.Where(item => item.IsSuccess)
                    .Select(objectName =>
                        new RealEstateImage(
                            request.RealEstateId,
                            objectName)), cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}