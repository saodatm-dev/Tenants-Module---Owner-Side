using Core.Application.Abstractions.Messaging;

namespace Building.Application.Amenities.Remove;

public sealed record RemoveAmenityCommand(Guid Id) : ICommand;
