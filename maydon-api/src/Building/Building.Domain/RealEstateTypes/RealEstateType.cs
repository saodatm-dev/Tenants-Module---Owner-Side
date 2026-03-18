using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Building.Domain.RealEstateTypes.Events;
using Core.Domain.Entities;
using Core.Domain.Languages;

namespace Building.Domain.RealEstateTypes;

[Table("real_estate_types", Schema = AssemblyReference.Instance)]
public sealed class RealEstateType : Entity
{
	private RealEstateType() { }
	public RealEstateType(
		string typeName,
		IEnumerable<LanguageValue> names,
		IEnumerable<LanguageValue>? descriptions,
		string? iconUrl = null,
		bool showBuildingSuggestion = false,
		bool showFloorSuggestion = false,
		bool canHaveUnits = false,
		bool canHaveMeters = false,
		bool canHaveFloors = false) : base()
	{
		TypeName = typeName;
		IconUrl = iconUrl;
		ShowBuildingSuggestion = showBuildingSuggestion;
		ShowFloorSuggestion = showFloorSuggestion;
		CanHaveUnits = canHaveUnits;
		CanHaveMeters = canHaveMeters;
		CanHaveFloors = canHaveFloors;
		Raise(new UpsertRealEstateTypeDomainEvent(this.Id, names, descriptions));
	}

	[MaxLength(50)]
	public string TypeName { get; private set; }
	[MaxLength(500)]
	public string? IconUrl { get; private set; }
	public bool ShowBuildingSuggestion { get; private set; }
	public bool ShowFloorSuggestion { get; private set; }
	public bool CanHaveUnits { get; private set; }
	public bool CanHaveMeters { get; private set; }
	public bool CanHaveFloors { get; private set; }
	public bool IsActive { get; private set; } = true;

	public RealEstateType Update(
		string typeName,
		IEnumerable<LanguageValue> names,
		IEnumerable<LanguageValue>? descriptions,
		string? iconUrl = null,
		bool showBuildingSuggestion = false,
		bool showFloorSuggestion = false,
		bool canHaveUnits = false,
		bool canHaveMeters = false,
		bool canHaveFloors = false)
	{
		TypeName = typeName;
		IconUrl = iconUrl;
		ShowBuildingSuggestion = showBuildingSuggestion;
		ShowFloorSuggestion = showFloorSuggestion;
		CanHaveUnits = canHaveUnits;
		CanHaveMeters = canHaveMeters;
		CanHaveFloors = canHaveFloors;
		Raise(new UpsertRealEstateTypeDomainEvent(this.Id, names, descriptions));
		return this;
	}
	public RealEstateType Remove()
	{
		Raise(new RemoveRealEstateTypeDomainEvent(this));
		return this;
	}
	public RealEstateType Activate()
	{
		IsActive = true;
		return this;
	}
	public RealEstateType Deactivate()
	{
		IsActive = false;
		return this;
	}
	public RealEstateType EnableBuildingSuggestion()
	{
		this.ShowBuildingSuggestion = true;
		return this;
	}
	public RealEstateType DisableBuildingSuggestion()
	{
		this.ShowBuildingSuggestion = false;
		return this;
	}
	public RealEstateType EnableFloorSuggestion()
	{
		this.ShowFloorSuggestion = true;
		return this;
	}
	public RealEstateType DisableFloorSuggestion()
	{
		this.ShowFloorSuggestion = false;
		return this;
	}
	public RealEstateType EnableUnits()
	{
		this.CanHaveUnits = true;
		return this;
	}
	public RealEstateType DisableUnits()
	{
		this.CanHaveUnits = false;
		return this;
	}
	public RealEstateType EnableMeters()
	{
		this.CanHaveMeters = true;
		return this;
	}

	public RealEstateType DisableMeters()
	{
		this.CanHaveMeters = false;
		return this;
	}
	public RealEstateType EnableFloors()
	{
		this.CanHaveFloors = true;
		return this;
	}

	public RealEstateType DisableFloors()
	{
		this.CanHaveFloors = false;
		return this;
	}
}
