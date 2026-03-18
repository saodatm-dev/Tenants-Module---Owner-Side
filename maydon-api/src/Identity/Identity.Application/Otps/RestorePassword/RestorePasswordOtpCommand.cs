using Core.Application.Abstractions.Messaging;

namespace Identity.Application.Otps.RestorePassword;

public sealed record RestorePasswordOtpCommand(string PhoneNumber) : ICommand;
