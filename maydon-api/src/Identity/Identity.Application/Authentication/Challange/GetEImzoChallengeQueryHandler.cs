using Core.Application.Abstractions.Messaging;
using Core.Domain.Results;
using Identity.Application.Core.Abstractions.Services.EImzo;

namespace Identity.Application.Authentication.Challange;

internal sealed class GetEImzoChallengeQueryHandler(
	IEImzoService eImzoService)
	: IQueryHandler<GetEImzoChallengeQuery, ChallengeResponse>
{
	public async Task<Result<ChallengeResponse>> Handle(GetEImzoChallengeQuery query, CancellationToken cancellationToken)
	{
		return await eImzoService.ChallengeAsync(cancellationToken);
	}
}
