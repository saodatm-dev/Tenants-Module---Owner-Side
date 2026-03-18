using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Building.Domain.RealEstates;
using Building.Domain.Units;
using Core.Domain.Entities;

namespace Building.Domain.RealEstateDelegations;

// Прав на управление недвижимостью

[Table("real_estate_delegations", Schema = AssemblyReference.Instance)]
public sealed class RealEstateDelegation : Entity
{
	private RealEstateDelegation() { }
	public RealEstateDelegation(
		Guid ownerId,
		Guid? agentId,
		Guid realEstateId,
		Guid? UnitId = null,
		DateOnly? validFrom = null,
		DateOnly? validUntil = null,
		Guid? contractId = null,
		string? contractNumber = null,
		long? commissionPercent = null,
		long? commissionFixed = null,
		short? maxLeaseMonths = null,                               // Максимальный срок аренды в месяцах
		long? maxLeaseAmount = null,                                // Максимальная сумма аренды в тийинах
		bool requireOwnerApproval = false) : base()                 // Требуется одобрение владельца
	{
		OwnerId = ownerId;
		AgentId = agentId;
		RealEstateId = realEstateId;
		ValidFrom = validFrom;
		ValidUntil = validUntil;
		ContractId = contractId;
		ContractNumber = contractNumber;
		CommissionPercent = commissionPercent;
		CommissionFixed = commissionFixed;
		MaxLeaseMonths = maxLeaseMonths;
		MaxLeaseAmount = maxLeaseAmount;
		RequireOwnerApproval = requireOwnerApproval;
	}

	public Guid OwnerId { get; private set; }
	public Guid? AgentId { get; private set; }
	public Guid RealEstateId { get; private set; }
	public Guid? UnitId { get; private set; }
	public DateOnly? ValidFrom { get; private set; }
	public DateOnly? ValidUntil { get; private set; }
	public Guid? ContractId { get; private set; }
	[MaxLength(100)]
	public string? ContractNumber { get; private set; }
	public long? CommissionPercent { get; private set; }                    // В процентах, например 5% = 5
	public long? CommissionFixed { get; private set; }                      // В тийинах, например 1000 сум  = 100000 

	// Ограничения
	public short? MaxLeaseMonths { get; private set; }                      // Максимальный срок аренды в месяцах
	public long? MaxLeaseAmount { get; private set; }                       // Максимальная сумма аренды в тийинах
	public bool RequireOwnerApproval { get; private set; }                  // Требуется одобрение владельца
	public DelegationStatus Status { get; private set; } = DelegationStatus.Active;
	[MaxLength(1000)]
	public string? Notes { get; private set; }
	public DateTime? RevokedAt { get; private set; }
	[MaxLength(500)]
	public string? RevokedReason { get; private set; }
	public RealEstate RealEstate { get; private set; }
	public Unit? Unit { get; private set; }

	public RealEstateDelegation UpdateNotes(string? notes)
	{
		Notes = notes;
		return this;
	}
	public RealEstateDelegation Activate()
	{
		this.Status = DelegationStatus.Active;
		return this;
	}
	public RealEstateDelegation Suspended()
	{
		this.Status = DelegationStatus.Suspended;
		return this;
	}
	public RealEstateDelegation Revoke(
		DateTime revokedAt,
		string reason)
	{
		if (Status == DelegationStatus.Revoked)
			return this;

		Status = DelegationStatus.Revoked;
		RevokedAt = revokedAt;
		RevokedReason = reason;
		return this;
	}
}
