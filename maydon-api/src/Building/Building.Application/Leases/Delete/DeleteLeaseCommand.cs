using Core.Application.Abstractions.Messaging;

namespace Building.Application.Leases.Delete;

public sealed record DeleteLeaseCommand(Guid Id) : ICommand;
