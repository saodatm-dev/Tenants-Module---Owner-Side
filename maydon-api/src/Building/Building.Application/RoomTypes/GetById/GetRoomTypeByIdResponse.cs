using Core.Domain.Languages;

namespace Building.Application.RoomTypes.GetById;

public sealed record GetRoomTypeByIdResponse(
	Guid Id,
	IEnumerable<LanguageValue> Translates);
