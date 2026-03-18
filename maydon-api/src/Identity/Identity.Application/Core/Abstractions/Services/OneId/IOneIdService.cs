using Core.Domain.Results;

namespace Identity.Application.Core.Abstractions.Services.OneId;

public interface IOneIdService
{
	ValueTask<Result<OneIdAccessTokenResponse>> AuthorizationAsync(Guid sessionId, CancellationToken cancellationToken);
	ValueTask<Result<OneIdResponse>> GetAsync(Guid accessToken, CancellationToken cancellationToken);
}
