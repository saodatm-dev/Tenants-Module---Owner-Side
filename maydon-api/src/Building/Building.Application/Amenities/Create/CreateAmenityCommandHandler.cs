using Building.Application.Core.Abstractions.Data;
using Building.Domain.Amenities;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.Amenities.Create;

internal sealed class CreateAmenityCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IBuildingDbContext dbContext) : ICommandHandler<CreateAmenityCommand, Guid>
{
	public async Task<Result<Guid>> Handle(CreateAmenityCommand command, CancellationToken cancellationToken)
	{
		if (!await dbContext.AmenityCategories.AsNoTracking().AnyAsync(item => item.Id == command.AmenityCategoryId, cancellationToken))
			return Result.Failure<Guid>(sharedViewLocalizer.NotFound(nameof(CreateAmenityCommand.AmenityCategoryId)));

		var item = new Amenity(
			command.AmenityCategoryId,
			command.IconUrl,
			command.Translates);

		await dbContext.Amenities.AddAsync(item, cancellationToken);

		await dbContext.SaveChangesAsync(cancellationToken);

		return item.Id;
	}
}
