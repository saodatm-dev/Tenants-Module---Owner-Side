using Core.Application.Abstractions.Messaging;
using Core.Domain.ValueObjects;

namespace Building.Application.Leases.Update;

public sealed record UpdateLeaseCommand(
    Guid Id,
    Guid OwnerId,
    Guid ClientId,
    DateOnly StartDate,
    short PaymentDay,
    IReadOnlyList<UpdateLeaseItemDto> Items,
    Guid? AgentId = null,
    Guid? ContractId = null,
    string? ContractNumber = null,
    DateOnly? EndDate = null) : ICommand<Guid>;

public sealed record UpdateLeaseItemDto(
    Guid ListingId,
    Guid RealEstateId,
    Money MonthlyRent,
    Guid? Id = null,
    Guid? RealEstateUnitId = null,
    Money DepositAmount = default,
    bool IsMetersIncluded = true,
    IEnumerable<Guid>? MeterIds = null);
