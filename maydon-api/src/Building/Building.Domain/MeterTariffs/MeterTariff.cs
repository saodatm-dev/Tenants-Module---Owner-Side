using System.ComponentModel.DataAnnotations.Schema;
using Building.Domain.MeterTypes;
using Core.Domain.Entities;

namespace Building.Domain.MeterTariffs;
/// <summary>
/// Represents a tariff configuration for a specific type of meter, including pricing and validity period information.
/// Supports Uzbekistan's tiered pricing with seasonal adjustments and social norm limits.
/// </summary>
/// <remarks>A MeterTariff defines the price per unit and optional fixed fee for a meter type, along with the
/// period during which the tariff is valid. Instances are typically associated with a meter type and may be used to
/// determine billing rates for metered consumption. This class is immutable except through the Update method.</remarks>

[Table("meter_tariffs", Schema = AssemblyReference.Instance)]
public sealed class MeterTariff : Entity
{
	private MeterTariff() { }
	public MeterTariff(
		Guid meterTypeId,
		DateOnly validFrom,
		DateOnly? validUntil,
		long price,                                             // Цена за единицу в тийинах
		MeterTariffType type = MeterTariffType.Individual,
		bool isActual = false,
		decimal? minLimit = null,
		decimal? maxLimit = null,
		long? fixedPrice = null,                                // Фиксированная плата в тийинах
		Season season = Season.All,                             // Сезонность тарифа
		decimal? socialNormLimit = null) : base()                // Лимит социальной нормы
	{

		MeterTypeId = meterTypeId;
		ValidFrom = validFrom;
		ValidUntil = validUntil;
		Price = price;
		Type = type;
		IsActual = isActual;
		MinLimit = minLimit;
		MaxLimit = maxLimit;
		FixedPrice = fixedPrice;
		Season = season;
		SocialNormLimit = socialNormLimit;
	}
	public Guid MeterTypeId { get; private set; }
	public DateOnly ValidFrom { get; private set; }
	public DateOnly? ValidUntil { get; private set; }
	public long Price { get; private set; }
	public MeterTariffType Type { get; private set; }
	[Column(TypeName = "numeric(18,2)")]
	public decimal? MinLimit { get; private set; }
	[Column(TypeName = "numeric(18,2)")]
	public decimal? MaxLimit { get; private set; }
	public long? FixedPrice { get; private set; }
	public bool IsActual { get; private set; }
	public Season Season { get; private set; } = Season.All;
	[Column(TypeName = "numeric(18,2)")]
	public decimal? SocialNormLimit { get; private set; }
	public MeterType MeterType { get; private set; }

	public MeterTariff Update(
		Guid meterTypeId,
		DateOnly validFrom,
		DateOnly? validUntil,
		long price,
		MeterTariffType type = MeterTariffType.Individual,
		bool isActual = false,
		decimal? minLimit = null,
		decimal? maxLimit = null,
		long? fixedPrice = null,
		Season season = Season.All,
		decimal? socialNormLimit = null)
	{
		MeterTypeId = meterTypeId;
		ValidFrom = validFrom;
		ValidUntil = validUntil;
		Price = price;
		Type = type;
		IsActual = isActual;
		MinLimit = minLimit;
		MaxLimit = maxLimit;
		FixedPrice = fixedPrice;
		Season = season;
		SocialNormLimit = socialNormLimit;
		return this;
	}
}
