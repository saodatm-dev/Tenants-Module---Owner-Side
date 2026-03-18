using System.ComponentModel.DataAnnotations;
using Core.Application.Abstractions.Messaging;

namespace Building.Application.Rooms.GetById;

public sealed record GetRoomByIdQuery([property: Required] Guid Id) : IQuery<GetRoomByIdResponse>;
