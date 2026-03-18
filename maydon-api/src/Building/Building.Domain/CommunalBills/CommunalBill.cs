using System.ComponentModel.DataAnnotations.Schema;
using Building.Domain.MeterReadings;
using Building.Domain.MeterTariffs;
using Building.Domain.RealEstates;
using Core.Domain.Entities;

namespace Building.Domain.CommunalBills;

/// <summary>
/// Коммунальный счёт - автоматически рассчитанный на основе показаний и тарифов
/// </summary>
[Table("communal_bills", Schema = AssemblyReference.Instance)]
public sealed class CommunalBill : Entity
{
	private CommunalBill() { }
	public CommunalBill(
		Guid realEstateId,
		Guid meterReadingId,
		Guid meterTariffId,
		DateOnly billingPeriodStart,
		DateOnly billingPeriodEnd,
		decimal consumption,
		long totalAmount,                   // Общая сумма в тийинах
		long? fixedAmount = null,           // Фиксированная часть в тийинах
		string? note = null) : base()
	{
		RealEstateId = realEstateId;
		MeterReadingId = meterReadingId;
		MeterTariffId = meterTariffId;
		BillingPeriodStart = billingPeriodStart;
		BillingPeriodEnd = billingPeriodEnd;
		Consumption = consumption;
		TotalAmount = totalAmount;
		FixedAmount = fixedAmount;
		Note = note;
		Status = BillStatus.Pending;
	}

	public Guid RealEstateId { get; private set; }
	public Guid MeterReadingId { get; private set; }
	public Guid MeterTariffId { get; private set; }
	public DateOnly BillingPeriodStart { get; private set; }
	public DateOnly BillingPeriodEnd { get; private set; }
	[Column(TypeName = "numeric(18,2)")]
	public decimal Consumption { get; private set; }
	public long TotalAmount { get; private set; }
	public long? FixedAmount { get; private set; }
	public long PaidAmount { get; private set; }
	public BillStatus Status { get; private set; }
	public string? Note { get; private set; }

	// Navigation
	public RealEstate RealEstate { get; private set; }
	public MeterReading MeterReading { get; private set; }
	public MeterTariff MeterTariff { get; private set; }
	public ICollection<CommunalPayment> Payments { get; private set; } = [];

	public CommunalBill MarkAsPaid(long paidAmount)
	{
		PaidAmount += paidAmount;
		Status = PaidAmount >= TotalAmount ? BillStatus.Paid : BillStatus.PartiallyPaid;
		return this;
	}

	public CommunalBill MarkAsOverdue()
	{
		if (Status == BillStatus.Pending || Status == BillStatus.PartiallyPaid)
			Status = BillStatus.Overdue;
		return this;
	}

	public CommunalBill Cancel()
	{
		Status = BillStatus.Cancelled;
		return this;
	}
}
