using Core.Application.Abstractions.Messaging;

namespace Building.Application.RealEstates.Remove;

public sealed record RemoveRealEstateCommand(Guid Id) : ICommand;
