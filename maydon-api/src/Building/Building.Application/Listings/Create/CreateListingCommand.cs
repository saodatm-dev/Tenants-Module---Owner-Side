using Building.Domain.Listings;
using Core.Application.Abstractions.Messaging;
using Core.Domain.Languages;

namespace Building.Application.Listings.Create;

public sealed record CreateListingCommand(
	Guid RealEstateId,
	IEnumerable<Guid> ListingCategoryIds,
	IEnumerable<Guid>? FloorIds,
	IEnumerable<Guid>? RoomIds,
	IEnumerable<Guid>? UnitIds,
	IEnumerable<Guid>? AmenityIds = null,
	decimal? PriceForMonth = null,
	decimal? PricePerSquareMeter = null,
	string? Description = null,
	string? Title = null,
	Guid? RentalPurposeId = null,
	MinLeaseTerm? MinLeaseTerm = null,
	UtilityPaymentType? UtilityPaymentType = null,
	DateOnly? NextAvailableDate = null,
	List<LanguageValue>? DescriptionTranslates = null) : ICommand<Guid>;

