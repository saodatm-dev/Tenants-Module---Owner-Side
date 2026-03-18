using Core.Application.Abstractions.Messaging;

namespace Building.Application.Moderations.RealEstates.Block;

public sealed record BlockModerationRealEstateCommand(Guid Id) : ICommand<Guid>;
