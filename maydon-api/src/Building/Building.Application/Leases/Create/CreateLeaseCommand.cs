using Core.Application.Abstractions.Messaging;

namespace Building.Application.Leases.Create;

public sealed record CreateLeaseCommand(
    Guid OwnerId,
    Guid ClientId,
    DateOnly StartDate,
    short PaymentDay,
    IReadOnlyList<CreateLeaseItemDto> Items,
    Guid? AgentId = null,
    Guid? ContractId = null,
    string? ContractNumber = null,
    DateOnly? EndDate = null) : ICommand<Guid>;

public sealed record CreateLeaseItemDto(
    Guid ListingId,
    Guid RealEstateId,
    decimal MonthlyRent,
    Guid? RealEstateUnitId = null,
    decimal? DepositAmount = null,
    bool IsMetersIncluded = true,
    IEnumerable<Guid>? MeterIds = null);
