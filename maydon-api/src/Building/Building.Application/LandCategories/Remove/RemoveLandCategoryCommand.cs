using System.ComponentModel.DataAnnotations;
using Core.Application.Abstractions.Messaging;

namespace Building.Application.LandCategories.Remove;

public sealed record RemoveLandCategoryCommand([property: Required] Guid Id) : ICommand;
