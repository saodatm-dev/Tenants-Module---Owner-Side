using Building.Application.Core.Abstractions.Data;
using Building.Application.MeterTypes.Update;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.MeterTariffs.Remove;

internal sealed class RemoveMeterTariffCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IBuildingDbContext dbContext) : ICommandHandler<RemoveMeterTariffCommand, Guid>
{
	public async Task<Result<Guid>> Handle(RemoveMeterTariffCommand command, CancellationToken cancellationToken)
	{
		var maybeItem = await dbContext.MeterTariffs.FirstOrDefaultAsync(item => item.Id == command.Id, cancellationToken);
		if (maybeItem is null)
			return Result.Failure<Guid>(sharedViewLocalizer.NotFound(nameof(UpdateMeterTypeCommand.Names)));

		dbContext.MeterTariffs.Remove(maybeItem);

		await dbContext.SaveChangesAsync(cancellationToken);

		return maybeItem.Id;
	}
}
