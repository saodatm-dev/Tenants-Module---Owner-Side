using Core.Application.Abstractions.Messaging;

namespace Building.Application.RoomTypes.Remove;

public sealed record RemoveRoomTypeCommand(Guid Id) : ICommand;
