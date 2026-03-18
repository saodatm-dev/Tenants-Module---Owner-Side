using Common.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Domain.Languages;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Common.Application.Currencies.GetById;

internal sealed class GetCurrencyByIdQueryHandler(ICommonDbContext dbcontext) : IQueryHandler<GetCurrencyByIdQuery, GetCurrencyByIdResponse>
{
	public async Task<Result<GetCurrencyByIdResponse>> Handle(GetCurrencyByIdQuery query, CancellationToken cancellationToken)
	{
		return await dbcontext.CurrencyTranslates
			.Include(item => item.Currency)
			.AsNoTracking()
			.Where(item => item.CurrencyId == query.Id)
			.GroupBy(item => item.Currency)
			.Select(item => new GetCurrencyByIdResponse(
				item.Key.Id,
				item.Key.Code,
				item.ToList()
					.Select(translate =>
						new LanguageValue(
							translate.LanguageId,
							translate.LanguageShortCode,
							translate.Value))
			)).FirstOrDefaultAsync(cancellationToken);
	}
}
