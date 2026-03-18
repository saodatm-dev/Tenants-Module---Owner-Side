using Common.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Domain.Languages;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Common.Application.Banks.GetById;

internal sealed class GetBankByIdQueryHandler(ICommonDbContext dbContext) : IQueryHandler<GetBankByIdQuery, GetBankByIdResponse>
{
	public async Task<Result<GetBankByIdResponse>> Handle(GetBankByIdQuery request, CancellationToken cancellationToken)
	{
		return await dbContext.Banks
			.IgnoreQueryFilters([IApplicationDbContext.TranslateFilter])
			.Where(item => item.Id == request.Id)
			.Include(item => item.Translates)
			.Select(item => new GetBankByIdResponse(
				item.Id,
				item.Mfo,
				item.Tin,
				item.PhoneNumber,
				item.Email,
				item.Website,
				item.Address,
				item.Translates.Select(translate =>
					new LanguageValue(
						translate.LanguageId,
						translate.LanguageShortCode,
						translate.Value))
				))
			.FirstOrDefaultAsync(cancellationToken);
	}
}
