using System.ComponentModel.DataAnnotations;
using Core.Application.Abstractions.Messaging;

namespace Building.Application.RoomTypes.GetById;

public sealed record GetRoomTypeByIdQuery([property: Required] Guid Id) : IQuery<GetRoomTypeByIdResponse>;
