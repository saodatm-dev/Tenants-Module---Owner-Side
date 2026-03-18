using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Providers;
using Core.Domain.Results;
using Identity.Application.Core.Abstractions.Cryptors;
using Identity.Application.Core.Abstractions.Data;
using Identity.Application.Core.Options;
using Identity.Domain.UserStates;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Identity.Application.Authentication.Registration.PhoneNumber;

internal sealed class PhoneNumberRegistrationCommandHandler(
    ISharedViewLocalizer sharedViewLocalizer,
    IDateTimeProvider dateTimeProvider,
    IIdentityDbContext dbContext,
    ICryptor cryptor,
    IOptionsMonitor<ApplicationOptions> options) : ICommandHandler<PhoneNumberRegistrationCommand, string>
{
    private const string DateTimeFormat = "dd.MM.yyyy HH:mm";
    public async Task<Result<string>> Handle(PhoneNumberRegistrationCommand command, CancellationToken cancellationToken)
    {
        // clean phone number
        var phoneNumber = command.PhoneNumber.Replace('+', ' ').Trim();

        var user = await dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.PhoneNumber == phoneNumber, cancellationToken);

        if (user is not null)
            return Result.Failure<string>(sharedViewLocalizer.UserAlreadyExists(command.PhoneNumber));

        var otpResult = await CheckOtpAsync(phoneNumber, command.Code, cancellationToken);
        if (otpResult.IsFailure)
            return Result.Failure<string>(otpResult.Error);

        var userState = new UserState(
            phoneNumber,
            dateTimeProvider.UtcNow.AddMinutes(options.CurrentValue.UserStateExpiredTimeInMinutes));

        await dbContext.UserStates.AddAsync(userState, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        return cryptor.EncryptUserState(userState.Id);

    }
    private async ValueTask<Result> CheckOtpAsync(string phoneNumber, string code, CancellationToken cancellationToken)
    {
        var otp = await dbContext.Otps.FirstOrDefaultAsync(item => item.PhoneNumber == phoneNumber && item.Status == Domain.Otps.OtpStatus.Waiting, cancellationToken);
        if (otp is null)
            return Result.Failure(sharedViewLocalizer.InvalidOtpCode(nameof(PhoneNumberRegistrationCommand.PhoneNumber)));

        var result = otp.Try(code, dateTimeProvider.UtcNow);
        if (result.IsSuccess)
        {
            dbContext.Otps.Remove(otp.Received());
            return result;
        }

        var error = Error.None;

        // is blocked 
        if (otp.Status == Domain.Otps.OtpStatus.Block && otp.NextAvailableTime is not null)
            error = sharedViewLocalizer.PhoneNumberBlockedUntilTime(phoneNumber, $"{otp.NextAvailableTime.Value.ToString(DateTimeFormat)}");

        // is incorrect
        error = sharedViewLocalizer.InvalidOtpCode(code);

        dbContext.Otps.Update(otp);

        return Result.Failure(error);
    }
}
