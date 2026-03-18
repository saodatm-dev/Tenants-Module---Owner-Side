using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Providers;
using Core.Domain.Results;
using Identity.Application.Core.Abstractions.Data;
using Identity.Application.Core.Abstractions.Services.Otp;

namespace Identity.Application.Otps.RestorePassword;

internal sealed class RestorePasswordOtpCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IDateTimeProvider dateTimeProvider,
	IIdentityDbContext dbContext,
	IOtpService otpService) :
		BaseOtpCommandHandler(sharedViewLocalizer, dateTimeProvider, dbContext, otpService),
		ICommandHandler<RestorePasswordOtpCommand>
{
	public async Task<Result> Handle(RestorePasswordOtpCommand command, CancellationToken cancellationToken) =>
		await SendAsync(command.PhoneNumber, Domain.Otps.OtpType.RestorePassword, cancellationToken);
}
