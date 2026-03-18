using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Building.Domain.Complexes.Events;
using Building.Domain.ComplexImages;
using Core.Domain.Entities;
using Core.Domain.Languages;
using NetTopologySuite.Geometries;

namespace Building.Domain.Complexes;

[Table("complexes", Schema = AssemblyReference.Instance)]
public sealed class Complex : Entity
{
	private Complex() { }
	public Complex(
		Guid regionId,
		Guid districtId,
		string name,
		IEnumerable<LanguageValue>? descriptions,
		bool isCommercial,
		bool isLiving,
		Point? location = null,
		string? address = null,
		IEnumerable<string>? images = null) : base()
	{
		RegionId = regionId;
		DistrictId = districtId;
		Name = name;
		IsCommercial = isCommercial;
		IsLiving = isLiving;
		Location = location;
		Address = address;

		Raise(new CreateOrUpdateComplexDomainEvent(this.Id, descriptions, images));
	}
	public Guid RegionId { get; private set; }
	public Guid DistrictId { get; private set; }
	[Required]
	[MaxLength(200)]
	public string Name { get; private set; }
	public Point? Location { get; private set; }
	[MaxLength(500)]
	public string? Address { get; private set; }
	public bool IsCommercial { get; private set; }          // type Commercial
	public bool IsLiving { get; private set; }              // type Living
	public bool IsActive { get; private set; } = true;
	public ICollection<Buildings.Building> Buildings { get; private set; }
	public ICollection<ComplexImage> Images { get; private set; }
	public ICollection<ComplexTranslate> Descriptions { get; private set; }

	public Complex Update(
		Guid regionId,
		Guid districtId,
		string name,
		IEnumerable<LanguageValue>? descriptions,
		bool isCommercial,
		bool isLiving,
		Point? location = null,
		string? address = null,
		IEnumerable<string>? images = null)
	{
		RegionId = regionId;
		DistrictId = districtId;
		Name = name;
		IsCommercial = isCommercial;
		IsLiving = isLiving;
		Location = location;
		Address = address;

		Raise(new CreateOrUpdateComplexDomainEvent(this.Id, descriptions, images));

		return this;
	}

	public Complex Remove()
	{
		Raise(new RemoveComplexDomainEvent(this.Id));
		return this;
	}
	public Complex Activate()
	{
		this.IsActive = true;
		return this;
	}
	public Complex Deactivate()
	{
		this.IsActive = false;
		return this;
	}
}
