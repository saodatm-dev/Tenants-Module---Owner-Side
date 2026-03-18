using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Building.Domain.MeterTariffs;
using Building.Domain.MeterTypes.Events;
using Core.Domain.Entities;
using Core.Domain.Languages;

namespace Building.Domain.MeterTypes;
/// <summary>
/// КОММУНАЛЬНЫЕ УСЛУГИ 
/// </summary>

[Table("meter_types", Schema = AssemblyReference.Instance)]
public sealed class MeterType : Entity
{
	private MeterType() { }
	public MeterType(
		IEnumerable<LanguageValue> names,
		IEnumerable<LanguageValue> units,
		string? icon = null) : base()
	{
		Icon = icon;
		Raise(new UpsertMeterTypeDomainEvent(
			this.Id,
			names,
			units));
	}

	[MaxLength(500)]
	public string? Icon { get; private set; }
	public bool IsActive { get; private set; } = true;
	public ICollection<MeterTariff> MeterTariffs { get; private set; } = [];
	public ICollection<MeterTypeTranslate> Translates { get; private set; } = [];

	public MeterType Update(
		IEnumerable<LanguageValue> names,
		IEnumerable<LanguageValue> units,
		string? icon = null)
	{
		Icon = icon;
		Raise(new UpsertMeterTypeDomainEvent(
			this.Id,
			names,
			units));

		return this;
	}
	public MeterType Remove()
	{
		Raise(new RemoveMeterTypeDomainEvent(this.Id));
		return this;
	}
	public MeterType Activate()
	{
		IsActive = true;
		return this;
	}
	public MeterType Deactivate()
	{
		IsActive = false;
		return this;
	}
}
