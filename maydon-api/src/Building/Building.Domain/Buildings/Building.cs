using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Building.Domain.BuildingImages;
using Building.Domain.Buildings.Events;
using Building.Domain.Complexes;
using Building.Domain.Floors;
using Building.Domain.Statuses;
using Core.Domain.Entities;
using Core.Domain.Languages;
using NetTopologySuite.Geometries;

namespace Building.Domain.Buildings;

[Table("buildings", Schema = AssemblyReference.Instance)]
public sealed class Building : Entity
{
	private Building() { }
	public Building(
		Guid? regionId,
		Guid? districtId,
		string number,
		bool isCommercial,
		bool isLiving,
		short? totalArea = null,
		short? floorsCount = null,
		Complex? complex = null,
		Point? location = null,
		string? address = null,
		IEnumerable<LanguageValue>? descriptions = null,
		IEnumerable<string>? images = null) : base()
	{
		RegionId = regionId;
		DistrictId = districtId;
		Number = number;
		IsCommercial = isCommercial;
		IsLiving = isLiving;
		FloorsCount = floorsCount;
		TotalArea = totalArea;
		Location = location;
		Address = address;

		if (complex is not null)
		{
			ComplexId = complex.Id;
			RegionId ??= complex.RegionId;
			DistrictId ??= complex.DistrictId;
			Location ??= complex.Location;
			Address ??= complex.Address;
		}

		Raise(new UpsertBuildingDomainEvent(this.Id, descriptions, images));
	}
	public Guid? OwnerId { get; private set; }
	public Guid? ComplexId { get; private set; }
	public Guid? RegionId { get; private set; }
	public Guid? DistrictId { get; private set; }
	[Required]
	[MaxLength(50)]
	public string Number { get; private set; }
	public bool IsCommercial { get; private set; }          // type Commercial
	public bool IsLiving { get; private set; }              // type Living	
	public short? TotalArea { get; private set; }
	public short? FloorsCount { get; private set; }
	public Point? Location { get; private set; }
	[MaxLength(500)]
	public string? Address { get; private set; }
	public Status Status { get; private set; } = Status.Active;
	public Complex? Complex { get; private set; }
	public ICollection<Floor> Floors { get; private set; }
	public ICollection<BuildingTranslate> Descriptions { get; private set; }
	public ICollection<BuildingImage> Images { get; private set; }

	public Building Update(
		Guid? regionId,
		Guid? districtId,
		string number,
		bool isCommercial,
		bool isLiving,
		short? totalArea = null,
		short? floorsCount = null,
		Complex? complex = null,
		Point? location = null,
		string? address = null,
		IEnumerable<LanguageValue>? descriptions = null,
		IEnumerable<string>? images = null)
	{
		RegionId = regionId;
		DistrictId = districtId;
		Number = number;
		IsCommercial = isCommercial;
		IsLiving = isLiving;
		Location = location;
		Address = address;
		TotalArea = totalArea;
		FloorsCount = floorsCount;

		if (complex is not null)
		{
			ComplexId = complex.Id;
			RegionId ??= complex.RegionId;
			DistrictId ??= complex.DistrictId;
			Location ??= complex.Location;
			Address ??= complex.Address;
		}

		Raise(new UpsertBuildingDomainEvent(this.Id, descriptions, images));

		return this;
	}
	public Building Remove()
	{
		Raise(new RemoveBuildingDomainEvent(this.Id));
		return this;
	}

	public Building Update(Guid? ownerId)
	{
		OwnerId = ownerId;
		return this;
	}
	public Building Activate()
	{
		this.Status = Status.Active;
		return this;
	}
	public Building Deactivate()
	{
		this.Status = Status.Inactive;
		return this;
	}
}

