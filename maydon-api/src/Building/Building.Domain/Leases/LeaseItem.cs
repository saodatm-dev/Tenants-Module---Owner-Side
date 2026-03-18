using System.ComponentModel.DataAnnotations.Schema;
using Building.Domain.RealEstates;
using Core.Domain.Entities;
using Core.Domain.ValueObjects;

namespace Building.Domain.Leases;

[Table("lease_items", Schema = AssemblyReference.Instance)]
public sealed class LeaseItem : Entity
{
    private LeaseItem() { }

    public LeaseItem(
        Guid leaseId,
        Guid listingId,
        Guid realEstateId,
        Guid? realEstateUnitId,
        Money monthlyRent,
        Money depositAmount = default,
        bool isMetersIncluded = true,
        IEnumerable<Guid>? meterIds = null) : base()
    {
        LeaseId = leaseId;
        ListingId = listingId;
        RealEstateId = realEstateId;
        RealEstateUnitId = realEstateUnitId;
        MonthlyRent = monthlyRent;
        DepositAmount = depositAmount;
        IsMetersIncluded = isMetersIncluded;
        MeterIds = meterIds;
    }

    public Guid LeaseId { get; private set; }
    public Guid ListingId { get; private set; }
    public Guid RealEstateId { get; private set; }
    public Guid? RealEstateUnitId { get; private set; }
    public Money MonthlyRent { get; private set; }
    public Money DepositAmount { get; private set; }
    public bool IsMetersIncluded { get; private set; }
    public IEnumerable<Guid>? MeterIds { get; private set; }

    public Lease Lease { get; private set; } = null!;
    public RealEstate? RealEstate { get; private set; }

    public LeaseItem Update(
        Guid listingId,
        Guid realEstateId,
        Guid? realEstateUnitId,
        Money monthlyRent,
        Money depositAmount,
        bool isMetersIncluded,
        IEnumerable<Guid>? meterIds)
    {
        ListingId = listingId;
        RealEstateId = realEstateId;
        RealEstateUnitId = realEstateUnitId;
        MonthlyRent = monthlyRent;
        DepositAmount = depositAmount;
        IsMetersIncluded = isMetersIncluded;
        MeterIds = meterIds;
        return this;
    }
}
