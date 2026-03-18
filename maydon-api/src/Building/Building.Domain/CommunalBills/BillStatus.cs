namespace Building.Domain.CommunalBills;

/// <summary>
/// Статус коммунального счёта
/// </summary>
public enum BillStatus
{
	Pending,        // Ожидает оплаты
	PartiallyPaid,  // Частично оплачен
	Paid,           // Оплачен
	Overdue,        // Просрочен
	Cancelled       // Отменён
}
