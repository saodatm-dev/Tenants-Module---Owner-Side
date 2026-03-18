using Building.Application.Core.Abstractions.Data;
using Building.Domain.Leases;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Core.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.Leases.Create;

internal sealed class CreateLeaseCommandHandler(
    ISharedViewLocalizer sharedViewLocalizer,
    IBuildingDbContext dbContext) : ICommandHandler<CreateLeaseCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateLeaseCommand command, CancellationToken cancellationToken)
    {
        var lease = new Lease(
            command.OwnerId,
            command.AgentId,
            command.ClientId,
            command.ContractId,
            command.ContractNumber,
            command.StartDate,
            command.EndDate,
            command.PaymentDay);

        foreach (var itemDto in command.Items)
        {
            if (!await dbContext.RealEstates
                    .AsNoTracking()
                    .IgnoreQueryFilters()
                    .AnyAsync(r => r.Id == itemDto.RealEstateId, cancellationToken))
                return Result.Failure<Guid>(sharedViewLocalizer.NotFound(nameof(itemDto.RealEstateId)));

            if (itemDto.RealEstateUnitId != null &&
                !await dbContext.Units
                    .IgnoreQueryFilters()
                    .AsNoTracking()
                    .AnyAsync(u => u.Id == itemDto.RealEstateUnitId, cancellationToken))
                return Result.Failure<Guid>(sharedViewLocalizer.NotFound(nameof(itemDto.RealEstateUnitId)));

            var leaseItem = new LeaseItem(
                lease.Id,
                itemDto.ListingId,
                itemDto.RealEstateId,
                itemDto.RealEstateUnitId,
                Money.FromSom(itemDto.MonthlyRent),
                itemDto.DepositAmount.HasValue ? Money.FromSom(itemDto.DepositAmount.Value) : default,
                itemDto.IsMetersIncluded,
                itemDto.MeterIds);

            lease.AddItem(leaseItem);
        }

        await dbContext.Leases.AddAsync(lease, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        return lease.Id;
    }
}
