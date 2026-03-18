using Building.Application.Core.Abstractions.Data;
using Building.Domain.MeterTariffs;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.MeterTariffs.Create;

internal sealed class CreateMeterTariffCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IBuildingDbContext dbContext) : ICommandHandler<CreateMeterTariffCommand, Guid>
{
	public async Task<Result<Guid>> Handle(CreateMeterTariffCommand command, CancellationToken cancellationToken)
	{
		if (!await dbContext.MeterTypes
			.AsNoTracking()
			.AnyAsync(item => item.Id == command.MeterTypeId, cancellationToken))
			return Result.Failure<Guid>(sharedViewLocalizer.InvalidValue(nameof(CreateMeterTariffCommand.MeterTypeId)));

		var item = new MeterTariff(
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
			command.SocialNormLimit);

		await dbContext.MeterTariffs.AddAsync(item, cancellationToken);

		await dbContext.SaveChangesAsync(cancellationToken);

		return item.Id;
	}
}
