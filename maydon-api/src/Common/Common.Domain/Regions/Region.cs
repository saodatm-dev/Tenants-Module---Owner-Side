using System.ComponentModel.DataAnnotations.Schema;
using Common.Domain.Regions.Events;
using Core.Domain.Entities;
using Core.Domain.Languages;

namespace Common.Domain.Regions;

[Table("regions", Schema = AssemblyReference.Instance)]
public sealed class Region : Entity
{
	private Region() { }
	public Region(IEnumerable<LanguageValue> translates) : base()
	{
		LanguageValues = translates;
		Raise(new UpsertRegionDomainEvent(this.Id, translates));
	}
	public short Order { get; private set; }

	[NotMapped]
	public IEnumerable<LanguageValue> LanguageValues { get; } = new List<LanguageValue>();

	public ICollection<RegionTranslate> Translates { get; }
		= new List<RegionTranslate>();


	public Region Update(IEnumerable<LanguageValue> translates)
	{
		Raise(new UpsertRegionDomainEvent(this.Id, translates));
		return this;
	}
	public Region SetOrder(short order)
	{
		this.Order = order;
		return this;
	}

	// to remove regions translates
	public Region Remove()
	{
		Raise(new RemoveRegionDomainEvent(this.Id));
		return this;
	}
}
