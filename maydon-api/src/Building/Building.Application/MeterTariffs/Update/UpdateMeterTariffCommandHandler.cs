using Building.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.MeterTariffs.Update;

internal sealed class UpdateMeterTariffCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IBuildingDbContext dbContext) : ICommandHandler<UpdateMeterTariffCommand, Guid>
{
	public async Task<Result<Guid>> Handle(UpdateMeterTariffCommand command, CancellationToken cancellationToken)
	{
		var maybeItem = await dbContext.MeterTariffs.FirstOrDefaultAsync(item => item.Id == command.Id, cancellationToken);
		if (maybeItem is null)
			return Result.Failure<Guid>(sharedViewLocalizer.NotFound(nameof(UpdateMeterTariffCommand.Id)));

		if (!await dbContext.MeterTypes
			.AsNoTracking()
			.AnyAsync(item => item.Id == command.MeterTypeId, cancellationToken))
			return Result.Failure<Guid>(sharedViewLocalizer.InvalidValue(nameof(UpdateMeterTariffCommand.MeterTypeId)));

		dbContext.MeterTariffs.Update(
			maybeItem.Update(
				command.MeterTypeId,
				command.ValidFrom,
				command.ValidTo,
				command.Price,
				command.Type,
				command.IsActual,
				command.MinLimit,
				command.MaxLimit,
				command.FixedPrice,
				command.Season,
				command.SocialNormLimit));

		await dbContext.SaveChangesAsync(cancellationToken);

		return maybeItem.Id;
	}
}
