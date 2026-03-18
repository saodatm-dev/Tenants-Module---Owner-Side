using Building.Application.Core.Abstractions.Data;
using Building.Domain.MeterTypes;
using Core.Application.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.MeterTypes.Create;

internal sealed class CreateMeterTypeCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IBuildingDbContext dbContext) : ICommandHandler<CreateMeterTypeCommand, Guid>
{
	public async Task<Result<Guid>> Handle(CreateMeterTypeCommand command, CancellationToken cancellationToken)
	{
		if (await dbContext.MeterTypeTranslates
			.AsNoTracking()
			.IgnoreQueryFilters([IApplicationDbContext.TranslateFilter])
			.AnyAsync(item => command.Names.Select(n => n.Value).Contains(item.Value), cancellationToken))
			return Result.Failure<Guid>(sharedViewLocalizer.AlreadyExists(nameof(CreateMeterTypeCommand.Names)));

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

		var item = new MeterType(
			command.Names,
			command.Description,
			command.Icon);

		await dbContext.MeterTypes.AddAsync(item, cancellationToken);

		await dbContext.SaveChangesAsync(cancellationToken);

		return item.Id;
	}
}
