using Core.Application.Abstractions.Messaging;

namespace Building.Application.Moderations.RealEstates.Cancel;

public sealed record CancelModerationRealEstateCommand(Guid Id) : ICommand<Guid>;
