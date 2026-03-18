using System.Web;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Identity.Application.Core.Abstractions.Cryptors;
using Identity.Application.Core.Abstractions.Data;

using Microsoft.EntityFrameworkCore;

namespace Identity.Application.Invitations.GetByKey;

internal sealed class GetInvitationByKeyQueryHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	ICryptor cryptor,
	IIdentityDbContext dbContext) : IQueryHandler<GetInvitationByKeyQuery, GetInvitationByKeyResponse>
{
	public async Task<Result<GetInvitationByKeyResponse>> Handle(GetInvitationByKeyQuery request, CancellationToken cancellationToken)
	{
		var key = HttpUtility.UrlDecode(request.Key, System.Text.Encoding.UTF8);

		var invitationId = cryptor.DecryptInvitation(key);

		if (invitationId == Guid.Empty)
		{
			return Result.Failure<GetInvitationByKeyResponse>(sharedViewLocalizer.InvitationNotFound(nameof(GetInvitationByKeyQuery.Key)));
		}

		return await dbContext.Invitations
			.Include(item => item.Recipient)
			.Include(item => item.Role)
			.Where(item => item.Id == invitationId && item.Status == Domain.Invitations.InvitationStatus.Sent)
			.Select(item => new GetInvitationByKeyResponse(
				item.Id,
				item.Content))
			.AsNoTracking()
			.FirstOrDefaultAsync(cancellationToken);
	}
}
