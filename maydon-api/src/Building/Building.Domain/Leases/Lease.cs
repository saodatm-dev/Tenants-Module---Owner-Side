using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Domain.Entities;

namespace Building.Domain.Leases;

[Table("leases", Schema = AssemblyReference.Instance)]
public sealed class Lease : Entity
{
	private readonly List<LeaseItem> _items = [];

	private Lease() { }

	public Lease(
		Guid ownerId,
		Guid? agentId,
		Guid clientId,
		Guid? contractId,
		string? contractNumber,
		DateOnly startDate,
		DateOnly? endDate,
		short paymentDay,
		IEnumerable<LeaseItem>? items = null) : base()
	{
		OwnerId = ownerId;
		AgentId = agentId;
		ClientId = clientId;
		ContractId = contractId;
		ContractNumber = contractNumber;
		StartDate = startDate;
		EndDate = endDate;
		PaymentDay = paymentDay;
		Status = LeaseStatus.Pending;

		if (items is not null)
			_items.AddRange(items);
	}

	public Guid OwnerId { get; private set; }
	public Guid? AgentId { get; private set; }
	public Guid ClientId { get; private set; }
	public Guid? ContractId { get; private set; }
	[MaxLength(100)]
	public string? ContractNumber { get; private set; }
	public DateOnly StartDate { get; private set; }
	public DateOnly? EndDate { get; private set; }
	public short PaymentDay { get; private set; }
	public LeaseStatus Status { get; private set; } = LeaseStatus.Pending;

	public IReadOnlyList<LeaseItem> Items => _items.AsReadOnly();

	public Lease Update(
		Guid ownerId,
		Guid? agentId,
		Guid clientId,
		Guid? contractId,
		string? contractNumber,
		DateOnly startDate,
		DateOnly? endDate,
		short paymentDay)
	{
		OwnerId = ownerId;
		AgentId = agentId;
		ClientId = clientId;
		ContractId = contractId;
		ContractNumber = contractNumber;
		StartDate = startDate;
		EndDate = endDate;
		PaymentDay = paymentDay;
		return this;
	}

	public void AddItem(LeaseItem item) => _items.Add(item);

	public void RemoveItem(LeaseItem item) => _items.Remove(item);

	public void ClearItems() => _items.Clear();

	public Lease Remove()
	{
		return this;
	}
}
