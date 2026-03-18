using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Domain.Entities;

namespace Building.Domain.CommunalBills;

/// <summary>
/// Коммунальный платёж - запись об оплате коммунального счёта
/// </summary>
[Table("communal_payments", Schema = AssemblyReference.Instance)]
public sealed class CommunalPayment : Entity
{
	private CommunalPayment() { }
	public CommunalPayment(
		Guid communalBillId,
		long amount,                        // Сумма платежа в тийинах
		DateOnly paymentDate,
		string? paymentMethod = null,
		string? transactionId = null,
		string? note = null) : base()
	{
		CommunalBillId = communalBillId;
		Amount = amount;
		PaymentDate = paymentDate;
		PaymentMethod = paymentMethod;
		TransactionId = transactionId;
		Note = note;
	}

	public Guid CommunalBillId { get; private set; }
	public long Amount { get; private set; }
	public DateOnly PaymentDate { get; private set; }
	[MaxLength(100)]
	public string? PaymentMethod { get; private set; }
	[MaxLength(200)]
	public string? TransactionId { get; private set; }
	[MaxLength(500)]
	public string? Note { get; private set; }

	// Navigation
	public CommunalBill CommunalBill { get; private set; }
}
