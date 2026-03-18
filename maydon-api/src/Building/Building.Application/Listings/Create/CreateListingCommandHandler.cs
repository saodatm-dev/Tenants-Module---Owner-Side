using Building.Application.Core.Abstractions.Data;
using Building.Domain.Floors;
using Building.Domain.Listings;
using Building.Domain.Rooms;
using Building.Domain.Units;
using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.Listings.Create;

internal sealed class CreateListingCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IExecutionContextProvider executionContextProvider,
	IBuildingDbContext dbContext) : ICommandHandler<CreateListingCommand, Guid>
{
	public async Task<Result<Guid>> Handle(CreateListingCommand command, CancellationToken cancellationToken)
	{
		// check listingCategories
		var listingCategories = await dbContext.ListingCategories.AsNoTracking().Where(item => command.ListingCategoryIds.Contains(item.Id)).ToListAsync(cancellationToken);
		if (!listingCategories.Any())
			return Result.Failure<Guid>(sharedViewLocalizer.ListingInvalidCategories(nameof(CreateListingCommand.ListingCategoryIds)));

		var realEstate = await dbContext.RealEstates
			.AsNoTrackingWithIdentityResolution()
			.Where(item => item.Id == command.RealEstateId && item.OwnerId == executionContextProvider.TenantId)
			.Include(item => item.Building)
			.ThenInclude(item => item != null ? item.Complex : null)
			.AsSplitQuery()
			.FirstOrDefaultAsync(cancellationToken);

		if (realEstate is null)
			return Result.Failure<Guid>(sharedViewLocalizer.ListingInvalidRealEstates(nameof(CreateListingCommand.RealEstateId)));

		IEnumerable<Floor> floors = null;
		if (command.FloorIds is not null && command.FloorIds.Any())
		{
			floors = await dbContext.Floors.AsNoTracking().Where(item => command.FloorIds.Contains(item.Id)).ToListAsync(cancellationToken);
			if (floors is null || !floors.Any() || floors.Count() != command.FloorIds.Count())
				return Result.Failure<Guid>(sharedViewLocalizer.FloorNotFound(nameof(CreateListingCommand.FloorIds)));
		}

		IEnumerable<Room> rooms = null;
		if (command.RoomIds is not null && command.RoomIds.Any())
		{
			rooms = await dbContext.Rooms.AsNoTracking().Where(item => command.RoomIds.Contains(item.Id)).ToListAsync(cancellationToken);
			if (rooms is null || !rooms.Any() || rooms.Count() != command.RoomIds.Count())
				return Result.Failure<Guid>(sharedViewLocalizer.FloorNotFound(nameof(CreateListingCommand.RoomIds)));
		}

		IEnumerable<Unit> units = null;
		if (command.UnitIds is not null && command.UnitIds.Any())
		{
			units = await dbContext.Units.AsNoTracking().Where(item => command.UnitIds.Contains(item.Id)).ToListAsync(cancellationToken);
			if (units is null || !units.Any() || units.Count() != command.UnitIds.Count())
				return Result.Failure<Guid>(sharedViewLocalizer.FloorNotFound(nameof(CreateListingCommand.UnitIds)));
		}

		// Validate and enrich description translates
		if (command.DescriptionTranslates is { Count: > 0 })
		{
			var languages = await dbContext.Languages.AsNoTracking().ToListAsync(cancellationToken);
			for (int i = 0; i < command.DescriptionTranslates.Count; i++)
			{
				var languageValue = command.DescriptionTranslates[i];
				var language = languages.Find(item => item.Id == languageValue.LanguageId);
				if (language is null)
					return Result.Failure<Guid>(sharedViewLocalizer.LanguageNotFound($"{nameof(CreateListingCommand.DescriptionTranslates)}.{languageValue.LanguageId}"));
				command.DescriptionTranslates[i] = languageValue with { LanguageShortCode = language.ShortCode };
			}
		}

		var item = new Listing(
			executionContextProvider.TenantId,
			realEstate.RenovationId,
			listingCategories.Select(item => item.Id),
			realEstate.Building?.Complex,
			realEstate.Building,
			realEstate,
			floors,
			rooms,
			units,
			(long?)(command.PriceForMonth != null ? command.PriceForMonth * 100 : null),
			(long?)(command.PricePerSquareMeter != null ? command.PricePerSquareMeter * 100 : null),
			command.Description,
			title: command.Title,
			AmenityIds: command.AmenityIds,
			rentalPurposeId: command.RentalPurposeId,
			minLeaseTerm: command.MinLeaseTerm,
			utilityPaymentType: command.UtilityPaymentType,
			nextAvailableDate: command.NextAvailableDate,
			descriptionTranslates: command.DescriptionTranslates);

		await dbContext.Listings.AddAsync(item, cancellationToken);

		await dbContext.SaveChangesAsync(cancellationToken);

		return item.Id;
	}
}

