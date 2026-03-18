using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Building.Domain.RealEstates;
using Building.Domain.Units;
using Core.Domain.Entities;

namespace Building.Domain.Floors;

[Table("floors", Schema = AssemblyReference.Instance)]
public sealed class Floor : Entity
{
	private Floor() { }
	public Floor(
		short number,
		FloorType? type = null,
		string? label = null,
		float? totalArea = null,
		float? ceilingHeight = null,
		string? plan = null,
		Guid? buildingId = null,
		Guid? realEstateId = null) : base()
	{
		ValidateParent(buildingId, realEstateId);
		BuildingId = buildingId;
		RealEstateId = realEstateId;
		Number = number;
		Type = type;
		Label = label;
		TotalArea = totalArea;
		CeilingHeight = ceilingHeight;
		Plan = plan;
	}
	public Guid? BuildingId { get; private set; }
	public Guid? RealEstateId { get; private set; }
	public short Number { get; private set; }
	public FloorType? Type { get; private set; }
	[MaxLength(20)]
	public string? Label { get; private set; }
	public float? TotalArea { get; private set; }
	public float? CeilingHeight { get; private set; }
	[MaxLength(200)]
	public string? Plan { get; private set; }
	public bool IsActive { get; private set; } = true;
	public Buildings.Building? Building { get; private set; }
	public RealEstate? RealEstate { get; private set; }
	public ICollection<RealEstate> RealEstates { get; private set; } = [];
	public ICollection<Unit> Units { get; private set; } = [];

	public Floor Update(
		short number,
		FloorType? type = null,
		string? label = null,
		float? totalArea = null,
		float? ceilingHeight = null,
		string? plan = null,
		Guid? buildingId = null,
		Guid? realEstateId = null)
	{
		ValidateParent(buildingId, realEstateId);
		BuildingId = buildingId;
		RealEstateId = realEstateId;
		Number = number;
		Type = type;
		Label = label;
		TotalArea = totalArea;
		CeilingHeight = ceilingHeight;
		Plan = plan;

		return this;
	}

	private static void ValidateParent(Guid? buildingId, Guid? realEstateId)
	{
		if (buildingId is not null && realEstateId is not null)
			throw new ArgumentException("Floor_CannotBelongToBoth");

		if (buildingId is null && realEstateId is null)
			throw new ArgumentException("Floor_MustHaveParent");
	}
	public Floor Activate()
	{
		IsActive = true;
		return this;
	}

	public Floor Deactivate()
	{
		IsActive = false;
		return this;
	}

	public Floor UpdatePlan(string objectName)
	{
		Plan = objectName;
		return this;
	}
}

