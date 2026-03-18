using Core.Application.Abstractions.Messaging;
using Identity.Domain.Otps;

namespace Identity.Application.Otps.Send;

public sealed record SendOtpCommand(string PhoneNumber, OtpType OtpType = OtpType.Registration) : ICommand;
