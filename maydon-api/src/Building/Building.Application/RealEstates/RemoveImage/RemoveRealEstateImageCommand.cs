using Core.Application.Abstractions.Messaging;

namespace Building.Application.RealEstates.RemoveImage;

public sealed record RemoveRealEstateImageCommand(Guid Id) : ICommand;
