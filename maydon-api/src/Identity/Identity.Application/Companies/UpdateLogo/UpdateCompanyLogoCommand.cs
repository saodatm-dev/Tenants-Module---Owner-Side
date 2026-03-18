using Core.Application.Abstractions.Messaging;

namespace Identity.Application.Companies.UpdateLogo;

public sealed record UpdateCompanyLogoCommand(string? ObjectName) : ICommand;
