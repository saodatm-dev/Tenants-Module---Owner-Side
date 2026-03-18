using Core.Application.Abstractions.Messaging;

namespace Building.Application.RealEstateTypes.Remove;

public sealed record RemoveRealEstateTypeCommand(Guid Id) : ICommand;
