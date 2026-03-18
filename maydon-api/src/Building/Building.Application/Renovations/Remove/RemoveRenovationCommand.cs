using System.ComponentModel.DataAnnotations;
using Core.Application.Abstractions.Messaging;

namespace Building.Application.Renovations.Remove;

public sealed record RemoveRenovationCommand([property: Required] Guid Id) : ICommand;
