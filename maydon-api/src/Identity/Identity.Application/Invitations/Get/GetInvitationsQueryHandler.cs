using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Application.Pagination;
using Core.Domain.Results;
using Identity.Application.Core.Abstractions.Data;
using Microsoft.EntityFrameworkCore;

namespace Identity.Application.Invitations.Get;

internal sealed class GetInvitationsQueryHandler(
	IExecutionContextProvider executionContextProvider,
	IIdentityDbContext dbContext) : IQueryHandler<GetInvitationsQuery, PagedList<GetInvitationsResponse>>
{
	public async Task<Result<PagedList<GetInvitationsResponse>>> Handle(GetInvitationsQuery request, CancellationToken cancellationToken)
	{
		var query = dbContext.Invitations
			.IgnoreQueryFilters([IApplicationDbContext.TenantIdFilter])
			.Include(item => item.Recipient)
			.Include(item => item.Role)
			.Where(item => 
				item.SenderId == executionContextProvider.TenantId || 
				item.RecipientId == executionContextProvider.UserId)
			.Where(item => !string.IsNullOrWhiteSpace(request.Filter)
				? (!string.IsNullOrEmpty(item.Recipient.FirstName) ? EF.Functions.Like(item.Recipient.FirstName.ToLower(), $"%{request.Filter.ToLowerInvariant()}%") : false) ||
				  (!string.IsNullOrEmpty(item.Recipient.LastName) ? EF.Functions.Like(item.Recipient.LastName.ToLower(), $"%{request.Filter.ToLowerInvariant()}%") : false) ||
				  (!string.IsNullOrEmpty(item.Recipient.MiddleName) ? EF.Functions.Like(item.Recipient.MiddleName.ToLower(), $"%{request.Filter.ToLowerInvariant()}%") : false)
				: true)
			.OrderByDescending(item => item.UpdatedAt)
			.ThenByDescending(item => item.CreatedAt)
			.Select(item => new GetInvitationsResponse(
				item.Id,
				item.ReceipientPhoneNumber,
				item.Recipient.FullName,
				item.Role.Name,
				item.ExpiredTime,
				item.Status,
				item.Reason))
			.AsNoTracking();

		int totalCount = await query.CountAsync(cancellationToken);

		var responsesPage = await query
			.Skip(request.Page)
			.Take(request.PageSize)
			.ToListAsync(cancellationToken);

		return new PagedList<GetInvitationsResponse>(responsesPage, request.Page, request.PageSize, totalCount);
	}
}
