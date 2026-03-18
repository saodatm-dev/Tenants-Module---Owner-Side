using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Building.Domain.Complexes;
using Building.Domain.Floors;
using Building.Domain.ListingAmenities;
using Building.Domain.Listings.Events;
using Building.Domain.RealEstates;
using Building.Domain.Renovations;
using Building.Domain.RentalPurposes;
using Building.Domain.Rooms;
using Building.Domain.Statuses;
using Building.Domain.Units;
using Core.Domain.Entities;
using Core.Domain.Languages;
using NetTopologySuite.Geometries;

namespace Building.Domain.Listings;

[Table("listings", Schema = AssemblyReference.Instance)]
public sealed class Listing : Entity
{
	private Listing() { }
	public Listing(
		Guid ownerId,
		Guid? renovationId,                                     // remont
		IEnumerable<Guid> listingCategoryIds,
		Complex? complex,
		Building.Domain.Buildings.Building? building,
		RealEstate realEstate,
		IEnumerable<Floor>? floors,
		IEnumerable<Room>? rooms,
		IEnumerable<Unit>? units,
		long? priceForMonth = null,                             // tiyin
		long? pricePerSquareMeter = null,
		string? description = null,
		string? title = null,
		IEnumerable<string>? Images = null,
		IEnumerable<Guid>? AmenityIds = null,
		Guid? rentalPurposeId = null,
		MinLeaseTerm? minLeaseTerm = null,
		UtilityPaymentType? utilityPaymentType = null,
		DateOnly? nextAvailableDate = null,
		IEnumerable<LanguageValue>? descriptionTranslates = null) : base()
	{
		OwnerId = ownerId;
		RenovationId = renovationId;
		CategoryIds = listingCategoryIds;
		ComplexId = complex?.Id;
		ComplexName = complex?.Name;
		BuildingId = building?.Id;
		BuildingNumber = building?.Number;

		RealEstateId = realEstate.Id;
		FloorIds = floors?.Select(item => item.Id);
		RoomIds = rooms?.Select(item => item.Id);
		UnitIds = units?.Select(item => item.Id);

		FloorNumbers = floors?.Select(item => item.Number);
		RoomsCount = rooms?.Count() ?? realEstate.Rooms?.Count;
		LivingArea = units is null && rooms is null && floors is null ? realEstate.LivingArea : null;

		TotalArea = units?.Sum(item => item.TotalArea ?? 0)
			?? rooms?.Sum(item => item.Area ?? 0)
			?? floors?.Sum(item => item.TotalArea ?? 0)
			?? realEstate.TotalArea
			?? 0;

		CeilingHeight = realEstate.CeilingHeight;

		PriceForMonth = priceForMonth;
		PricePerSquareMeter = pricePerSquareMeter;
		Description = description;
		Title = title;

		RentalPurposeId = rentalPurposeId;
		MinLeaseTerm = minLeaseTerm;
		UtilityPaymentType = utilityPaymentType;
		NextAvailableDate = nextAvailableDate;

		RegionId = realEstate.RegionId;
		DistrictId = realEstate.DistrictId;
		Location = realEstate.Location;
		Address = realEstate.Address;

		Raise(new UpsertListingDomainEvent(this, AmenityIds, descriptionTranslates));

		this.InModeration();
	}
	public Guid OwnerId { get; private set; }
	public Guid? RenovationId { get; private set; }
	public IEnumerable<Guid> CategoryIds { get; private set; }
	public Guid RealEstateId { get; private set; }
	public Guid? ComplexId { get; private set; }
	[MaxLength(200)]
	public string? ComplexName { get; private set; }
	public Guid? BuildingId { get; private set; }
	[MaxLength(50)]
	public string? BuildingNumber { get; private set; }
	public IEnumerable<Guid>? FloorIds { get; private set; }
	public IEnumerable<Guid>? RoomIds { get; private set; }
	public IEnumerable<Guid>? UnitIds { get; private set; }
	public IEnumerable<short>? FloorNumbers { get; private set; }
	public int? RoomsCount { get; private set; }
	public float? LivingArea { get; private set; }
	public float TotalArea { get; private set; }
	public float? CeilingHeight { get; private set; }

	public long? PriceForMonth { get; private set; }                    // tiyin
	public long? PricePerSquareMeter { get; private set; }              // tiyin	

	[MaxLength(2000)]
	public string? Description { get; private set; }
	[MaxLength(200)]
	public string? Title { get; private set; }

	public Guid? RentalPurposeId { get; private set; }
	public MinLeaseTerm? MinLeaseTerm { get; private set; }
	public UtilityPaymentType? UtilityPaymentType { get; private set; }

	public Guid? RegionId { get; private set; }
	public Guid? DistrictId { get; private set; }
	public Point? Location { get; private set; }
	[MaxLength(500)]
	public string? Address { get; private set; }
	public DateOnly? NextAvailableDate { get; private set; }
	public Status Status { get; private set; } = Status.Draft;
	public ModerationStatus ModerationStatus { get; private set; } = ModerationStatus.InModeration;
	[MaxLength(500)]
	public string Reason { get; private set; }

	public Renovation Renovation { get; private set; }
	public RentalPurpose? RentalPurpose { get; private set; }
	public Complex? Complex { get; private set; }
	public Buildings.Building? Building { get; private set; }
	public RealEstate RealEstate { get; private set; }
	public ICollection<ListingAmenity> ListingAmenities { get; private set; }

	public Listing Update(
		Guid? renovationId,                            // remont
		IEnumerable<Guid> listingCategoryIds,
		RealEstate realEstate,
		IEnumerable<Floor>? floors,
		IEnumerable<Room>? rooms,
		IEnumerable<Unit>? units,
		long? priceForMonth = null,                             // tiyin
		long? pricePerSquareMeter = null,
		string? description = null,
		string? title = null,
		IEnumerable<string>? Images = null,
		IEnumerable<Guid>? AmenityIds = null,
		Guid? rentalPurposeId = null,
		MinLeaseTerm? minLeaseTerm = null,
		UtilityPaymentType? utilityPaymentType = null,
		DateOnly? nextAvailableDate = null,
		IEnumerable<LanguageValue>? descriptionTranslates = null)
	{
		RenovationId = renovationId;
		CategoryIds = listingCategoryIds;
		RealEstateId = realEstate.Id;
		FloorIds = floors?.Select(item => item.Id);
		RoomIds = rooms?.Select(item => item.Id);
		UnitIds = units?.Select(item => item.Id);

		FloorNumbers = floors?.Select(item => item.Number);
		RoomsCount = rooms?.Count() ?? realEstate.Rooms?.Count;
		LivingArea = units is null && rooms is null && floors is null ? realEstate.LivingArea : null;

		TotalArea = units?.Sum(item => item.TotalArea ?? 0)
			?? rooms?.Sum(item => item.Area ?? 0)
			?? floors?.Sum(item => item.TotalArea ?? 0)
			?? realEstate.TotalArea
			?? 0;

		CeilingHeight = realEstate.CeilingHeight;

		PriceForMonth = priceForMonth;
		PricePerSquareMeter = pricePerSquareMeter;
		Description = description;
		Title = title;

		RentalPurposeId = rentalPurposeId;
		MinLeaseTerm = minLeaseTerm;
		UtilityPaymentType = utilityPaymentType;
		NextAvailableDate = nextAvailableDate;

		RegionId = realEstate.RegionId;
		DistrictId = realEstate.DistrictId;
		Location = realEstate.Location;
		Address = realEstate.Address;

		this.InModeration();

		Raise(new UpsertListingDomainEvent(this, AmenityIds, descriptionTranslates));
		return this;
	}

	public Listing Remove()
	{
		Raise(new RemoveListingDomainEvent(this.Id));
		return this;
	}

	public Listing SetNextAvailableDate(DateOnly? date)
	{
		NextAvailableDate = date;
		return this;
	}
	public Listing Activate()
	{
		this.Status = Status.Active;
		return this;
	}
	public Listing Deactivate()
	{
		this.Status = Status.Inactive;
		return this;
	}
	public bool IsInModeration() => ModerationStatus == ModerationStatus.InModeration;
	public bool IsAccept() => ModerationStatus == ModerationStatus.Accept;
	public bool IsCancel() => ModerationStatus == ModerationStatus.Cancel;
	public bool IsReject() => ModerationStatus == ModerationStatus.Reject;
	public bool IsBlocked() => ModerationStatus == ModerationStatus.Block;
	public Listing InModeration()
	{
		ModerationStatus = ModerationStatus.InModeration;
		Reason = string.Empty;
		return this;
	}
	public Listing Accept()
	{
		ModerationStatus = ModerationStatus.Accept;
		Status = Status.Active; // TODO remove it to prod

		Reason = string.Empty;
		return this;
	}
	public Listing Cancel()
	{
		ModerationStatus = ModerationStatus.Cancel;
		return this;
	}
	public Listing Reject(string reason)
	{
		ModerationStatus = ModerationStatus.Reject;
		Reason = reason;
		return this;
	}
	public Listing Block(string? reason)
	{
		ModerationStatus = ModerationStatus.Block;
		Reason = reason ?? string.Empty;
		return this;
	}
}
