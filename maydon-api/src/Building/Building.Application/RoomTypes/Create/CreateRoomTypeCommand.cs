using Core.Application.Abstractions.Messaging;
using Core.Domain.Languages;

namespace Building.Application.RoomTypes.Create;

public sealed record CreateRoomTypeCommand(List<LanguageValue> Translates) : ICommand<Guid>;
