using Core.Application.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Domain.Languages;
using Core.Domain.Results;
using Identity.Application.Core.Abstractions.Data;
using Microsoft.EntityFrameworkCore;

namespace Identity.Application.OtpContents.GetById;

internal sealed class GetOtpContentByIdQueryHandler(IIdentityDbContext dbContext) : IQueryHandler<GetOtpContentByIdQuery, GetOtpContentByIdQueryResponse>
{
	public async Task<Result<GetOtpContentByIdQueryResponse>> Handle(GetOtpContentByIdQuery request, CancellationToken cancellationToken)
	{
		return await dbContext.OtpContents
			.AsNoTracking()
			.Where(item => item.OtpType == request.OtpType)
			.IgnoreQueryFilters([IApplicationDbContext.TranslateFilter])
			.GroupBy(item => item.OtpType)
			.Select(item => new GetOtpContentByIdQueryResponse(
				item.Key,
				item.ToList().Select(translate =>
				new LanguageValue(
					translate.LanguageId,
					translate.LanguageShortCode,
					translate.Content))))
			.FirstOrDefaultAsync(cancellationToken);
	}
}
