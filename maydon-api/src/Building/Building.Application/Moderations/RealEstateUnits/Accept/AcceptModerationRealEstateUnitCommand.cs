using Core.Application.Abstractions.Messaging;

namespace Building.Application.Moderations.RealEstateUnits.Accept;

public sealed record AcceptModerationRealEstateUnitCommand(Guid Id) : ICommand<Guid>;
