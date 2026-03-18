using Core.Application.Abstractions.Messaging;

namespace Building.Application.Moderations.RealEstateUnits.Block;

public sealed record BlockModerationRealEstateUnitCommand(Guid Id) : ICommand<Guid>;
