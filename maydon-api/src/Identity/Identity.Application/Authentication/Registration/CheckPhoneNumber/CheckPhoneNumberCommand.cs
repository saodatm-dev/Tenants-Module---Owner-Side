using Core.Application.Abstractions.Messaging;

namespace Identity.Application.Authentication.Registration.CheckPhoneNumber;

public sealed record CheckPhoneNumberCommand(string PhoneNumber) : ICommand;
