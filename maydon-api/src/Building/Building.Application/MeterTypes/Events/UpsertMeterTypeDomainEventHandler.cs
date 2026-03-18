using Building.Application.Core.Abstractions.Data;
using Building.Domain.MeterTypes;
using Building.Domain.MeterTypes.Events;
using Core.Application.Abstractions.Data;
using Core.Domain.Events;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.MeterTypes.Events;

internal sealed class UpsertMeterTypeDomainEventHandler(IBuildingDbContext dbContext) : IDomainEventHandler<UpsertMeterTypeDomainEvent>
{
	public async ValueTask Handle(UpsertMeterTypeDomainEvent @event, CancellationToken cancellationToken)
	{
		var existTranslates = await dbContext.MeterTypeTranslates
			.IgnoreQueryFilters([IApplicationDbContext.TranslateFilter])
			.Where(item => item.MeterTypeId == @event.MeterTypeId)
			.ToListAsync(cancellationToken);

		if (existTranslates.Count > 0)
		{
			// update names
			foreach (var name in @event.Names)
			{
				var existTranslate = existTranslates.Find(item =>
					item.LanguageId == name.LanguageId && item.Field == MeterTypeField.Name);
				if (existTranslate is not null)
					dbContext.MeterTypeTranslates.Update(
						existTranslate.Update(
							MeterTypeField.Name,
							name.LanguageId,
							name.LanguageShortCode,
							name.Value));
				else
					await dbContext.MeterTypeTranslates.AddAsync(
						new MeterTypeTranslate(
							@event.MeterTypeId,
							MeterTypeField.Name,
							name.LanguageId,
							name.LanguageShortCode,
							name.Value),
						cancellationToken);
			}

			// update descriptions (units)
			foreach (var unit in @event.Units)
			{
				var existTranslate = existTranslates.Find(item =>
					item.LanguageId == unit.LanguageId && item.Field == MeterTypeField.Description);
				if (existTranslate is not null)
					dbContext.MeterTypeTranslates.Update(
						existTranslate.Update(
							MeterTypeField.Description,
							unit.LanguageId,
							unit.LanguageShortCode,
							unit.Value));
				else
					await dbContext.MeterTypeTranslates.AddAsync(
						new MeterTypeTranslate(
							@event.MeterTypeId,
							MeterTypeField.Description,
							unit.LanguageId,
							unit.LanguageShortCode,
							unit.Value),
						cancellationToken);
			}
		}
		else
		{
			// insert all names
			await dbContext.MeterTypeTranslates.AddRangeAsync(
				@event.Names
				.Select(item =>
					new MeterTypeTranslate(
						@event.MeterTypeId,
						MeterTypeField.Name,
						item.LanguageId,
						item.LanguageShortCode,
						item.Value)),
				cancellationToken);

			// insert all descriptions (units)
			await dbContext.MeterTypeTranslates.AddRangeAsync(
				@event.Units
				.Select(item =>
					new MeterTypeTranslate(
						@event.MeterTypeId,
						MeterTypeField.Description,
						item.LanguageId,
						item.LanguageShortCode,
						item.Value)),
				cancellationToken);
		}

		await ValueTask.CompletedTask;
	}
}
