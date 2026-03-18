using Core.Application.Abstractions.Messaging;

namespace Building.Application.Moderations.RealEstateUnits.Cancel;

public sealed record CancelModerationRealEstateUnitCommand(Guid Id) : ICommand<Guid>;
