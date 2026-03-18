using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Building.Domain.Floors;
using Building.Domain.RealEstates;
using Building.Domain.Renovations;
using Building.Domain.Rooms;
using Building.Domain.Statuses;
using Building.Domain.Units.Events;
using Core.Domain.Entities;

namespace Building.Domain.Units;

[Table("real_estate_units", Schema = AssemblyReference.Instance)]
public sealed class Unit : Entity
{
	private Unit() { }
	public Unit(
		Guid ownerId,
		Guid? realEstateId,
		Guid? realEstateTypeId,
		Floor? floor,
		Room? room,
		short? floorNumber,
		string? roomNumber,

		// Характеристики
		Guid? renovationId,                            // remont
		float totalArea,
		float? ceilingHeight = null,

		// Метаданные
		string? plan = null,
		IEnumerable<UnitCoordinate>? coordinates = null,
		IEnumerable<string>? images = null) : base()
	{
		OwnerId = ownerId;
		RealEstateId = realEstateId;
		RealEstateTypeId = realEstateTypeId;

		FloorId = floor?.Id;
		RoomId = room?.Id;
		FloorNumber = floor?.Number ?? floorNumber;
		RoomNumber = room?.Number ?? roomNumber;

		RenovationId = renovationId;
		TotalArea = totalArea;
		CeilingHeight = floor?.CeilingHeight ?? ceilingHeight;

		Plan = plan;
		Coordinates = coordinates?.ToList();
		Images = images?.ToList();

		this.InModeration();
	}
	public Guid OwnerId { get; private set; }
	public Guid? RealEstateId { get; private set; }
	public Guid? RealEstateTypeId { get; private set; }
	public Guid? FloorId { get; private set; }
	public Guid? RoomId { get; private set; }
	public Guid? RenovationId { get; private set; }
	public short? FloorNumber { get; private set; }
	[MaxLength(50)]
	public string? RoomNumber { get; private set; }
	public float? TotalArea { get; private set; }
	public float? CeilingHeight { get; private set; }
	public List<UnitCoordinate>? Coordinates { get; private set; }
	[MaxLength(200)]
	public string? Plan { get; private set; }
	public List<string>? Images { get; private set; }
	public Status Status { get; private set; } = Status.Draft;
	public ModerationStatus ModerationStatus { get; private set; } = ModerationStatus.InModeration;
	[MaxLength(500)]
	public string? Reason { get; private set; }
	public Renovation? Renovation { get; private set; }
	public RealEstate RealEstate { get; private set; }
	public Floor? Floor { get; private set; }
	public Room? Room { get; private set; }
	public Unit Update(
		Guid? realEstateTypeId,
		Floor? floor,
		Room? room,
		short? floorNumber,
		string? roomNumber,

		// Характеристики
		Guid? renovationId,                            // remont
		float totalArea,
		float? ceilingHeight = null,

		// Метаданные
		string? plan = null,
		IEnumerable<UnitCoordinate>? coordinates = null,
		IEnumerable<string>? images = null)
	{
		RealEstateTypeId = realEstateTypeId;
		FloorId = floor?.Id;
		RoomId = room?.Id;
		FloorNumber = floor?.Number ?? floorNumber;
		RoomNumber = room?.Number ?? roomNumber;

		RenovationId = renovationId;
		TotalArea = totalArea;
		CeilingHeight = floor?.CeilingHeight ?? ceilingHeight;

		RenovationId = renovationId;
		TotalArea = totalArea;
		CeilingHeight = ceilingHeight;

		Plan = plan;
		Coordinates = coordinates?.ToList();
		Images = images?.ToList();

		this.InModeration();

		return this;
	}

	public Unit Remove()
	{
		Raise(new RemoveUnitDomainEvent(this));
		return this;
	}
	public Unit Activate()
	{
		this.Status = Status.Active;
		return this;
	}
	public Unit Deactivate()
	{
		this.Status = Status.Inactive;
		return this;
	}
	public bool IsInModeration() => ModerationStatus == ModerationStatus.InModeration;
	public bool IsAccept() => ModerationStatus == ModerationStatus.Accept;
	public bool IsCancel() => ModerationStatus == ModerationStatus.Cancel;
	public bool IsReject() => ModerationStatus == ModerationStatus.Reject;
	public bool IsBlocked() => ModerationStatus == ModerationStatus.Block;
	public Unit InModeration()
	{
		ModerationStatus = ModerationStatus.InModeration;
		Reason = string.Empty;
		return this;
	}
	public Unit Accept()
	{
		ModerationStatus = ModerationStatus.Accept;
		Status = Status.Active; // TODO remove it to prod
		Reason = string.Empty;
		return this;
	}
	public Unit Cancel()
	{
		ModerationStatus = ModerationStatus.Cancel;
		return this;
	}
	public Unit Reject(string reason)
	{
		ModerationStatus = ModerationStatus.Reject;
		Reason = reason;
		return this;
	}
	public Unit Block()
	{
		ModerationStatus = ModerationStatus.Block;
		return this;
	}
}
