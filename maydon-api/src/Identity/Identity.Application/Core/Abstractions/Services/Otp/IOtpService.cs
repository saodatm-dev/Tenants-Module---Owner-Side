using Core.Domain.Results;

namespace Identity.Application.Core.Abstractions.Services.Otp;

public interface IOtpService
{
	ValueTask<Result> SendAsync(string phoneNumber, string content, CancellationToken cancellationToken = default);
}
