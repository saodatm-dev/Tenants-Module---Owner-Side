using System.ComponentModel.DataAnnotations;
using Core.Application.Abstractions.Messaging;

namespace Building.Application.RentalPurposes.Remove;

public sealed record RemoveRentalPurposeCommand([property: Required] Guid Id) : ICommand;
