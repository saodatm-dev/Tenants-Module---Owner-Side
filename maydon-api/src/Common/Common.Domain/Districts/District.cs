using System.ComponentModel.DataAnnotations.Schema;
using Common.Domain.Districts.Events;
using Common.Domain.Regions;
using Core.Domain.Entities;
using Core.Domain.Languages;

namespace Common.Domain.Districts;

[Table("districts", Schema = AssemblyReference.Instance)]
public sealed class District : Entity
{
	private District() { }
	public District(
		Guid regionId,
		IEnumerable<LanguageValue> translates) : base()
	{
		RegionId = regionId;
		LanguageValues = translates;
		Raise(new UpsertDistrictDomainEvent(this.Id, translates));
	}
	public Guid RegionId { get; private set; }
	public short Order { get; private set; }
	public ICollection<DistrictTranslate> Translates { get; } = new List<DistrictTranslate>();
	[NotMapped]
	public IEnumerable<LanguageValue> LanguageValues { get; } = new List<LanguageValue>();
	public Region Region { get; private set; }
	public District Update(
		Guid regionId,
		IEnumerable<LanguageValue> translates)
	{
		RegionId = regionId;
		Raise(new UpsertDistrictDomainEvent(this.Id, translates));
		return this;
	}
	public District SetOrder(short order)
	{
		this.Order = order;
		return this;
	}

	// to remove district translates
	public District Remove()
	{
		Raise(new RemoveDistrictDomainEvent(this.Id));
		return this;
	}
}
