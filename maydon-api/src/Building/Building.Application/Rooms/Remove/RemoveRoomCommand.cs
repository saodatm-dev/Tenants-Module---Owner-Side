using Core.Application.Abstractions.Messaging;

namespace Building.Application.Rooms.Remove;

public sealed record RemoveRoomCommand(Guid Id) : ICommand;
