using Core.Application.Abstractions.Messaging;
using Core.Domain.Languages;

namespace Building.Application.RoomTypes.Update;

public sealed record UpdateRoomTypeCommand(
	Guid Id,
	List<LanguageValue> Translates) : ICommand<Guid>;
