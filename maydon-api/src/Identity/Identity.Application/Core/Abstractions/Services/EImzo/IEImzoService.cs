using Core.Domain.Results;

namespace Identity.Application.Core.Abstractions.Services.EImzo;

public interface IEImzoService
{
	ValueTask<Result<PingResponse>> PingAsync(CancellationToken cancellationToken);
	ValueTask<Result<ChallengeResponse>> ChallengeAsync(CancellationToken cancellationToken);
	ValueTask<Result<AuthResponse>> AuthAsync(string pkcs7, CancellationToken cancellationToken);
	ValueTask<Result<MobileAuthInitResponse>> MobileAuthInitAsync(CancellationToken cancellationToken);
	ValueTask<Result<MobileAuthStatusResponse>> MobileAuthStatusAsync(string documentId, CancellationToken cancellationToken);
	ValueTask<Result<AuthResponse>> MobileAuthenticateAsync(string documentId, CancellationToken cancellationToken);
}
