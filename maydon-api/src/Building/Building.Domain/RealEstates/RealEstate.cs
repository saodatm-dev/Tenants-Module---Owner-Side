using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Building.Domain.Domain.RealEstates.Events;
using Building.Domain.Floors;
using Building.Domain.LandCategories;
using Building.Domain.ProductionTypes;
using Building.Domain.Meters;
using Building.Domain.RealEstateAmenities;
using Building.Domain.RealEstateDelegations;
using Building.Domain.RealEstateImages;
using Building.Domain.RealEstates.Events;
using Building.Domain.RealEstateTypes;
using Building.Domain.Renovations;
using Building.Domain.Rooms;
using Building.Domain.Statuses;
using Building.Domain.Units;
using Core.Domain.Entities;
using Core.Domain.Languages;
using NetTopologySuite.Geometries;

namespace Building.Domain.RealEstates;

[Table("real_estates", Schema = AssemblyReference.Instance)]
public sealed class RealEstate : Entity
{
	private RealEstate() { }
	public RealEstate(
		Guid ownerId,
		Guid realEstateTypeId,
		string? buildingNumber,
		short? floorNumber,
		string? number,

		// Характеристики
		Guid? renovationId,                            // remont
		Guid? landCategoryId,                          // категория земли
		Guid? productionTypeId,                        // тип производства
		string? cadastralNumber,
		float totalArea,
		float? livingArea = null,
		float? ceilingHeight = null,
		short? totalFloors = null,
		short? aboveFloors = null,
		short? belowFloors = null,
		short? roomsCount = null,
		Buildings.Building? building = null,
		Floor? floor = null,
		IEnumerable<UnitRequest>? units = null,
		IEnumerable<RoomValue>? rooms = null,

		//Адрес
		Guid? regionId = null,
		Guid? districtId = null,
		Point? location = null,
		string? address = null,

		// Метаданные
		string? plan = null,
		IEnumerable<LanguageValue>? descriptions = null,
		IEnumerable<string>? images = null,
		IEnumerable<Guid>? amenityIds = null) : base()
	{
		OwnerId = ownerId;
		RenovationId = renovationId;
		LandCategoryId = landCategoryId;
		ProductionTypeId = productionTypeId;
		RealEstateTypeId = realEstateTypeId;
		CadastralNumber = cadastralNumber;
		BuildingNumber = buildingNumber;
		FloorNumber = floorNumber;
		TotalFloors = totalFloors;
		AboveFloors = aboveFloors;
		BelowFloors = belowFloors;
		Number = number;
		TotalArea = totalArea;
		LivingArea = livingArea;
		RoomsCount = rooms?.Count() ?? roomsCount;
		CeilingHeight = ceilingHeight;
		RegionId = regionId;
		DistrictId = districtId;
		Location = location;
		Address = address;
		Plan = plan;
		// if building is not null then take from it
		if (building is not null)
		{
			Building = building;
			BuildingId = building.Id;
			RegionId = building.RegionId;
			DistrictId = building.DistrictId;
			Location = building.Location;
			Address = building.Address;
		}

		// if floor is not null then take from it
		if (floor is not null)
		{
			Floor = floor;
			FloorId = floor.Id;
			FloorNumber = floor.Number;
			CeilingHeight = floor.CeilingHeight;
		}

		Raise(new UpsertRealEstateDomainEvent(this, units, rooms, descriptions, images, amenityIds));

		this.InModeration();
	}
	public Guid OwnerId { get; private set; }
	public Guid RealEstateTypeId { get; private set; }
	public Guid? LandCategoryId { get; private set; }
	public Guid? ProductionTypeId { get; private set; }
	public Guid? RenovationId { get; private set; }
	[MaxLength(50)]
	public string? CadastralNumber { get; private set; }
	public Guid? BuildingId { get; private set; }
	[MaxLength(50)]
	public string? BuildingNumber { get; private set; }
	public Guid? FloorId { get; private set; }
	public short? FloorNumber { get; private set; }
	public short? TotalFloors { get; private set; }
	public short? AboveFloors { get; private set; }
	public short? BelowFloors { get; private set; }
	[MaxLength(50)]
	public string? Number { get; private set; }
	public float? TotalArea { get; private set; }
	public float? LivingArea { get; private set; }
	public float? CeilingHeight { get; private set; }
	public int? RoomsCount { get; private set; }
	public Guid? RegionId { get; private set; }
	public Guid? DistrictId { get; private set; }
	public Point? Location { get; private set; }
	[MaxLength(500)]
	public string? Address { get; private set; }
	[MaxLength(500)]
	public string? Plan { get; private set; }
	public ICollection<RealEstateTranslate>? Descriptions { get; private set; }
	public Status Status { get; private set; } = Status.Draft;
	public ModerationStatus ModerationStatus { get; private set; } = ModerationStatus.InModeration;
	[MaxLength(1000)]
	public string? Reason { get; private set; }
	public RealEstateType RealEstateType { get; private set; }
	public Renovation? Renovation { get; private set; }
	public LandCategory? LandCategory { get; private set; }
	public ProductionType? ProductionType { get; private set; }
	public Buildings.Building? Building { get; private set; }
	public Floor? Floor { get; private set; }
	public ICollection<Room>? Rooms { get; private set; }
	public ICollection<RealEstateImage>? Images { get; private set; }
	public ICollection<Unit> Units { get; private set; } = new HashSet<Unit>();
	public ICollection<Meter> Meters { get; private set; } = [];
	public ICollection<RealEstateAmenity> RealEstateAmenities { get; private set; }
	public PropertyCategory PropertyCategory { get; private set; } = PropertyCategory.Residential;
	public RealEstateDelegation RealEstateDelegation { get; private set; }


	public RealEstate Update(
		Guid realEstateTypeId,
		string? buildingNumber,
		short? floorNumber,
		string? number,

		// Характеристики
		Guid? renovationId,                            // remont
		Guid? landCategoryId,                          // категория земли
		Guid? productionTypeId,                        // тип производства
		string? cadastralNumber,
		float totalArea,
		float? livingArea = null,
		float? ceilingHeight = null,
		short? totalFloors = null,
		short? aboveFloors = null,
		short? belowFloors = null,
		short? roomsCount = null,
		Buildings.Building? building = null,
		Floor? floor = null,
		IEnumerable<UnitRequest>? units = null,
		IEnumerable<RoomValue>? rooms = null,

		//Адрес
		Guid? regionId = null,
		Guid? districtId = null,
		Point? location = null,
		string? address = null,

		// Метаданные
		string? plan = null,
		IEnumerable<LanguageValue>? descriptions = null,
		IEnumerable<string>? images = null,
		IEnumerable<Guid>? amenityIds = null)
	{
		RenovationId = renovationId;
		LandCategoryId = landCategoryId;
		ProductionTypeId = productionTypeId;
		RealEstateTypeId = realEstateTypeId;
		CadastralNumber = cadastralNumber;
		BuildingNumber = buildingNumber;
		FloorNumber = floorNumber;
		TotalFloors = totalFloors;
		AboveFloors = aboveFloors;
		BelowFloors = belowFloors;
		Number = number;
		TotalArea = totalArea;
		RoomsCount = rooms?.Count() ?? roomsCount;
		CeilingHeight = ceilingHeight;
		RegionId = regionId;
		DistrictId = districtId;
		Location = location;
		Address = address;
		Plan = plan;
		// if building is not null then take from it
		if (building is not null)
		{
			Building = building;
			BuildingId = building.Id;
			RegionId = building.RegionId;
			DistrictId = building.DistrictId;
			Location = building.Location;
			Address = building.Address;
		}

		// if floor is not null then take from it
		if (floor is not null)
		{
			Floor = floor;
			FloorId = floor.Id;
			FloorNumber = floor.Number;
			CeilingHeight = floor.CeilingHeight;
		}

		Raise(new UpsertRealEstateDomainEvent(this, units, rooms, descriptions, images, amenityIds));

		this.InModeration();

		return this;
	}
	public RealEstate Activate()
	{
		this.Status = Status.Active;
		return this;
	}
	public RealEstate Deactivate()
	{
		this.Status = Status.Inactive;
		return this;
	}
	public RealEstate Remove()
	{
		Raise(new RemoveRealEstateDomainEvent(this.Id));
		return this;
	}

	public bool IsInModeration() => ModerationStatus == ModerationStatus.InModeration;
	public bool IsAccept() => ModerationStatus == ModerationStatus.Accept;
	public bool IsCancel() => ModerationStatus == ModerationStatus.Cancel;
	public bool IsReject() => ModerationStatus == ModerationStatus.Reject;
	public bool IsBlocked() => ModerationStatus == ModerationStatus.Block;
	public RealEstate InModeration()
	{
		ModerationStatus = ModerationStatus.InModeration;
		Reason = string.Empty;
		return this;
	}
	public RealEstate Accept()
	{
		ModerationStatus = ModerationStatus.Accept;
		Status = Status.Active;
		Reason = string.Empty;
		return this;
	}
	public RealEstate Cancel()
	{
		ModerationStatus = ModerationStatus.Cancel;
		return this;
	}
	public RealEstate Reject(string reason)
	{
		ModerationStatus = ModerationStatus.Reject;
		Reason = reason;
		return this;
	}
	public RealEstate Block()
	{
		ModerationStatus = ModerationStatus.Block;
		return this;
	}
}
