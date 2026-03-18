using System.Security.Cryptography;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Providers;
using Core.Domain.Results;
using Identity.Application.Core.Abstractions.Data;
using Identity.Application.Core.Abstractions.Services.Otp;
using Microsoft.EntityFrameworkCore;

namespace Identity.Application.Otps.Send;

internal sealed class SendOtpCommandHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IDateTimeProvider dateTimeProvider,
	IIdentityDbContext dbContext,
	IOtpService otpService) : ICommandHandler<SendOtpCommand>
{
	private const string DateTimeFormat = "dd.MM.yyyy HH:mm";

	public async Task<Result> Handle(SendOtpCommand command, CancellationToken cancellationToken)
	{
		var previousOtp = await dbContext.Otps.AsNoTracking()
			.Where(item => item.PhoneNumber == command.PhoneNumber && item.Status == Domain.Otps.OtpStatus.Waiting)
			.OrderByDescending(item => item.CreatedAt)
			.FirstOrDefaultAsync(cancellationToken);

		if (previousOtp is not null)
		{
			var otpStatus = previousOtp.IsAvailable(dateTimeProvider.UtcNow);
			// check available time 
			if (otpStatus == Domain.Otps.OtpStatus.Waiting)
				return Result.Failure(sharedViewLocalizer.TryAfterTime(command.PhoneNumber, $"{previousOtp.NextAvailableTime?.AddHours(IDateTimeProvider.TashkentTimeDifference).ToString(DateTimeFormat)}"));

			if (otpStatus == Domain.Otps.OtpStatus.Block)
			{
				dbContext.Otps.Update(previousOtp.Block(dateTimeProvider.UtcNow));
				await dbContext.SaveChangesAsync(cancellationToken);

				return Result.Failure(sharedViewLocalizer.PhoneNumberBlockedUntilTime(command.PhoneNumber, $"{previousOtp.NextAvailableTime?.AddHours(IDateTimeProvider.TashkentTimeDifference).ToString(DateTimeFormat)}"));
			}

			dbContext.Otps.Remove(previousOtp.NotApplied());
		}

		var code = await CodeGeneratorAsync();

		var otp = new Domain.Otps.Otp(command.PhoneNumber, code, (ushort)(previousOtp is not null ? previousOtp.SentMessageCount + 1 : 1));

		var content = command.OtpType switch
		{
			Domain.Otps.OtpType.Registration => sharedViewLocalizer.RegistrationCodeContent(code),
			Domain.Otps.OtpType.RestorePassword => sharedViewLocalizer.RestorePasswordContent(code),
			_ => string.Empty
		};

		if (string.IsNullOrWhiteSpace(content))
			return Result.Failure(sharedViewLocalizer.InvalidValue(nameof(SendOtpCommand.OtpType)));

		var result = await otpService.SendAsync(command.PhoneNumber, content, cancellationToken);
		if (result.IsFailure)
			return result;

		otp.Sent(dateTimeProvider.UtcNow);

		await dbContext.Otps.AddAsync(otp, cancellationToken);

		await dbContext.SaveChangesAsync(cancellationToken);

		return Result.Success();
	}
	private async ValueTask<string> CodeGeneratorAsync()
	{
		var code = RandomNumberGenerator.GetInt32(0, 1000000).ToString("D6");
		if (await dbContext.Otps.AnyAsync(item => item.Code == code))
			return await CodeGeneratorAsync();

		return code;
	}
}
