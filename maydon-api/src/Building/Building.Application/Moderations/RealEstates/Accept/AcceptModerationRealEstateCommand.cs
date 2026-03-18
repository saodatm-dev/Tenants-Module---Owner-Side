using Core.Application.Abstractions.Messaging;

namespace Building.Application.Moderations.RealEstates.Accept;

public sealed record AcceptModerationRealEstateCommand(Guid Id) : ICommand<Guid>;
