using Common.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Common.Application.Languages.GetById;

internal sealed class GetLanguageByIdQueryHandler(ICommonDbContext dbContext) : IQueryHandler<GetLanguageByIdQuery, GetLanguageByIdResponse>
{
	public async Task<Result<GetLanguageByIdResponse>> Handle(GetLanguageByIdQuery request, CancellationToken cancellationToken)
	{
		return await dbContext.Languages
			.AsNoTracking()
			.Where(item => item.Id == request.Id)
			.Select(item => new GetLanguageByIdResponse(
				item.Id,
				item.Name,
				item.ShortCode,
				item.Order))
			.FirstOrDefaultAsync(cancellationToken);
	}
}
