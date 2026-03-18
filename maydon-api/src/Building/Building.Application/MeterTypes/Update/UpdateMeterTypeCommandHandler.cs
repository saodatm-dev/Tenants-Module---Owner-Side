using Building.Application.Core.Abstractions.Data;
using Building.Application.MeterTypes.Create;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.MeterTypes.Update;

internal sealed class UpdateMeterTypeCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IBuildingDbContext dbContext) : ICommandHandler<UpdateMeterTypeCommand, Guid>
{
	public async Task<Result<Guid>> Handle(UpdateMeterTypeCommand command, CancellationToken cancellationToken)
	{
		var maybeItem = await dbContext.MeterTypes.FirstOrDefaultAsync(item => item.Id == command.Id, cancellationToken);
		if (maybeItem is null)
			return Result.Failure<Guid>(sharedViewLocalizer.NotFound(nameof(UpdateMeterTypeCommand.Names)));

		// check names language ids
		if (!await dbContext.Languages
			.AsNoTracking()
			.AllAsync(item => command.Names.Select(n => n.LanguageId).Contains(item.Id), cancellationToken))
			return Result.Failure<Guid>(sharedViewLocalizer.InvalidValue(nameof(CreateMeterTypeCommand.Names)));

		// check descriptions language ids
		if (!await dbContext.Languages
			.AsNoTracking()
			.AllAsync(item => command.Description.Select(n => n.LanguageId).Contains(item.Id), cancellationToken))
			return Result.Failure<Guid>(sharedViewLocalizer.InvalidValue(nameof(CreateMeterTypeCommand.Description)));

		dbContext.MeterTypes.Update(maybeItem.Update(command.Names, command.Description, command.Icon));

		await dbContext.SaveChangesAsync(cancellationToken);

		return maybeItem.Id;
	}
}
