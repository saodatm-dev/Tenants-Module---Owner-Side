using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Building.Domain.Meters;
using Core.Domain.Entities;

namespace Building.Domain.MeterReadings;
/// <summary>
/// Показания счетчиков
/// </summary>
[Table("meter_readings", Schema = AssemblyReference.Instance)]
public sealed class MeterReading : Entity
{
	private MeterReading() { }
	public MeterReading(
		Guid meterId,
		Guid realEstateId,
		DateOnly readingDate,
		decimal value,
		decimal previousValue,
		decimal consumption,                  // Потребление = текущее - предыдущее
		bool isManual = true,               // isManual - вручную введенные показания		
		string? note = null) : base()
	{
		MeterId = meterId;
		RealEstateId = realEstateId;
		ReadingDate = readingDate;
		Value = value;
		PreviousValue = previousValue;
		Consumption = consumption;
		IsManual = isManual;
		Note = note;
	}
	public Guid MeterId { get; private set; }
	public Guid RealEstateId { get; private set; }
	public DateOnly ReadingDate { get; private set; }
	[Column(TypeName = "numeric(18,2)")]
	public decimal Value { get; private set; }
	[Column(TypeName = "numeric(18,2)")]
	public decimal PreviousValue { get; private set; }
	[Column(TypeName = "numeric(18,2)")]
	public decimal Consumption { get; private set; }
	public bool IsManual { get; private set; }
	[MaxLength(500)]
	public string? Note { get; private set; }
	public Meter Meter { get; private set; }

	public MeterReading Update(
		DateOnly readingDate,
		decimal value,
		decimal previousValue,
		decimal consumption,
		bool isManual,
		string? note = null)
	{
		ReadingDate = readingDate;
		Value = value;
		PreviousValue = previousValue;
		Consumption = consumption;
		IsManual = isManual;
		Note = note;
		return this;
	}
}
