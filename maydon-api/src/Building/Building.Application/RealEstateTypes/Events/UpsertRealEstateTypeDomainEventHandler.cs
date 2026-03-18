using Building.Application.Core.Abstractions.Data;
using Building.Domain.RealEstateTypes;
using Building.Domain.RealEstateTypes.Events;
using Core.Application.Abstractions.Data;
using Core.Domain.Events;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.RealEstateTypes.Events;

internal sealed class UpsertRealEstateTypeDomainEventHandler(IBuildingDbContext dbContext) : IDomainEventHandler<UpsertRealEstateTypeDomainEvent>
{
	public async ValueTask Handle(UpsertRealEstateTypeDomainEvent @event, CancellationToken cancellationToken)
	{
		await UpsertNames(@event, cancellationToken);

		await UpsertDescriptions(@event, cancellationToken);

		await ValueTask.CompletedTask;
	}
	private async ValueTask UpsertNames(UpsertRealEstateTypeDomainEvent @event, CancellationToken cancellationToken)
	{
		if (@event.Names is not null && @event.Names.Any())
		{
			var existTranslates = await dbContext.RealEstateTypeTranslates
				.IgnoreQueryFilters([IApplicationDbContext.TranslateFilter])
				.Where(item => item.RealEstateTypeId == @event.RealEstateTypeId && item.Field == RealEstateTypeField.Name)
				.ToListAsync(cancellationToken);

			if (existTranslates.Count > 0)
			{
				// update
				foreach (var translate in @event.Names)
				{
					var existTranslate = existTranslates.Find(item => item.LanguageId == translate.LanguageId);
					if (existTranslate is not null)
						dbContext.RealEstateTypeTranslates.Update(
							existTranslate.Update(
								Domain.RealEstateTypes.RealEstateTypeField.Name,
								translate.LanguageId,
								translate.LanguageShortCode,
								translate.Value));
					else
						await dbContext.RealEstateTypeTranslates.AddAsync(
							new RealEstateTypeTranslate(
								@event.RealEstateTypeId,
								Domain.RealEstateTypes.RealEstateTypeField.Name,
								translate.LanguageId,
								translate.LanguageShortCode,
								translate.Value),
							cancellationToken);
				}
			}
			else
			{
				await dbContext.RealEstateTypeTranslates.AddRangeAsync(
					@event.Names.Select(translate =>
						new RealEstateTypeTranslate(
							@event.RealEstateTypeId,
							Domain.RealEstateTypes.RealEstateTypeField.Name,
							translate.LanguageId,
							translate.LanguageShortCode,
							translate.Value))
					, cancellationToken);
			}
		}
	}
	private async ValueTask UpsertDescriptions(UpsertRealEstateTypeDomainEvent @event, CancellationToken cancellationToken)
	{
		if (@event.Descriptions is not null && @event.Descriptions.Any())
		{
			var existTranslates = await dbContext.RealEstateTypeTranslates
				.IgnoreQueryFilters([IApplicationDbContext.TranslateFilter])
				.Where(item => item.RealEstateTypeId == @event.RealEstateTypeId && item.Field == RealEstateTypeField.Description)
				.ToListAsync(cancellationToken);

			if (existTranslates.Count > 0)
			{
				// update
				foreach (var translate in @event.Descriptions)
				{
					var existTranslate = existTranslates.Find(item => item.LanguageId == translate.LanguageId);
					if (existTranslate is not null)
						dbContext.RealEstateTypeTranslates.Update(
							existTranslate.Update(
								RealEstateTypeField.Description,
								translate.LanguageId,
								translate.LanguageShortCode,
								translate.Value));
					else
						await dbContext.RealEstateTypeTranslates.AddAsync(
							new RealEstateTypeTranslate(
								@event.RealEstateTypeId,
								RealEstateTypeField.Description,
								translate.LanguageId,
								translate.LanguageShortCode,
								translate.Value),
							cancellationToken);
				}
			}
			else
			{
				await dbContext.RealEstateTypeTranslates.AddRangeAsync(
					@event.Descriptions.Select(translate =>
						new RealEstateTypeTranslate(
							@event.RealEstateTypeId,
							RealEstateTypeField.Description,
							translate.LanguageId,
							translate.LanguageShortCode,
							translate.Value))
					, cancellationToken);
			}
		}
	}
}
