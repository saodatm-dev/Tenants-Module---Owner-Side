using Core.Application.Abstractions.Messaging;
using Core.Domain.Results;
using Identity.Application.Core.Abstractions.Data;
using Microsoft.EntityFrameworkCore;

namespace Identity.Application.BankProperties.GetById;

internal sealed class GetBankPropertyByIdQueryHandler(IIdentityDbContext dbContext) : IQueryHandler<GetBankPropertyByIdQuery, GetBankPropertyByIdResponse>
{
	public async Task<Result<GetBankPropertyByIdResponse>> Handle(GetBankPropertyByIdQuery command, CancellationToken cancellationToken)
	{
		return await dbContext.BankProperties
			.AsNoTracking()
			.Where(item => item.Id == command.Id)
			.Select(item => new GetBankPropertyByIdResponse(
				item.Id,
				item.BankName,
				item.BankMFO,
				item.AccountNumber,
				item.IsMain,
				item.IsPublic))
			.FirstOrDefaultAsync(cancellationToken);
	}
}
