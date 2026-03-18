using Building.Application.Core.Abstractions.Data;
using Building.Domain.Leases;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.Leases.Update;

internal sealed class UpdateLeaseCommandHandler(
    ISharedViewLocalizer sharedViewLocalizer,
    IBuildingDbContext dbContext) : ICommandHandler<UpdateLeaseCommand, Guid>
{
    public async Task<Result<Guid>> Handle(UpdateLeaseCommand command, CancellationToken cancellationToken)
    {
        var lease = await dbContext.Leases
            .Include(l => l.Items)
            .FirstOrDefaultAsync(l => l.Id == command.Id, cancellationToken);

        if (lease is null)
            return Result.Failure<Guid>(sharedViewLocalizer.NotFound(nameof(UpdateLeaseCommand.Id)));

        lease.Update(
            command.OwnerId,
            command.AgentId,
            command.ClientId,
            command.ContractId,
            command.ContractNumber,
            command.StartDate,
            command.EndDate,
            command.PaymentDay);

        // Determine which items to keep, update, or remove
        var incomingIds = command.Items
            .Where(i => i.Id.HasValue)
            .Select(i => i.Id!.Value)
            .ToHashSet();

        // Remove items not in the incoming list
        var itemsToRemove = lease.Items
            .Where(existing => !incomingIds.Contains(existing.Id))
            .ToList();

        foreach (var item in itemsToRemove)
            lease.RemoveItem(item);

        foreach (var itemDto in command.Items)
        {
            if (!await dbContext.RealEstates.AsNoTracking().AnyAsync(
                    r => r.Id == itemDto.RealEstateId, cancellationToken))
                return Result.Failure<Guid>(sharedViewLocalizer.NotFound(nameof(itemDto.RealEstateId)));

            if (itemDto.RealEstateUnitId != null &&
                !await dbContext.Units.AsNoTracking().AnyAsync(
                    u => u.Id == itemDto.RealEstateUnitId, cancellationToken))
                return Result.Failure<Guid>(sharedViewLocalizer.NotFound(nameof(itemDto.RealEstateUnitId)));

            if (itemDto.Id.HasValue)
            {
                // Update existing item
                var existingItem = lease.Items.FirstOrDefault(i => i.Id == itemDto.Id.Value);
                existingItem?.Update(
                    itemDto.ListingId,
                    itemDto.RealEstateId,
                    itemDto.RealEstateUnitId,
                    itemDto.MonthlyRent,
                    itemDto.DepositAmount,
                    itemDto.IsMetersIncluded,
                    itemDto.MeterIds);
            }
            else
            {
                // Add new item
                var newItem = new LeaseItem(
                    lease.Id,
                    itemDto.ListingId,
                    itemDto.RealEstateId,
                    itemDto.RealEstateUnitId,
                    itemDto.MonthlyRent,
                    itemDto.DepositAmount,
                    itemDto.IsMetersIncluded,
                    itemDto.MeterIds);

                lease.AddItem(newItem);
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return lease.Id;
    }
}
