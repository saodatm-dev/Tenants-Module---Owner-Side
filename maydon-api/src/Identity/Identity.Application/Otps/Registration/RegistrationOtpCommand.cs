using Core.Application.Abstractions.Messaging;

namespace Identity.Application.Otps.Registration;

public sealed record RegistrationOtpCommand(string PhoneNumber) : ICommand;
