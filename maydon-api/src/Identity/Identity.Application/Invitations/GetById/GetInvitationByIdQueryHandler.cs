using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Domain.Results;
using Identity.Application.Core.Abstractions.Data;
using Microsoft.EntityFrameworkCore;

namespace Identity.Application.Invitations.GetById;

internal sealed class GetInvitationByIdQueryHandler(
	IExecutionContextProvider executionContextProvider,
	IIdentityDbContext dbContext) : IQueryHandler<GetInvitationByIdQuery, GetInvitationByIdResponse>
{
	public async Task<Result<GetInvitationByIdResponse>> Handle(GetInvitationByIdQuery request, CancellationToken cancellationToken)
	{
		return await dbContext.Invitations
			.IgnoreQueryFilters([IApplicationDbContext.TenantIdFilter])
			.Include(item => item.Recipient)
			.Include(item => item.Role)
			.Where(item => item.Id == request.Id)
			.Where(item => 
				item.SenderId == executionContextProvider.TenantId || 
				item.RecipientId == executionContextProvider.UserId)
			.Select(item => new GetInvitationByIdResponse(
				item.Id,
				item.ReceipientPhoneNumber,
				item.Recipient.FullName,
				item.Role.Name,
				item.ExpiredTime,
				item.Status,
				item.Reason))
			.AsNoTracking()
			.FirstOrDefaultAsync(cancellationToken);
	}
}
