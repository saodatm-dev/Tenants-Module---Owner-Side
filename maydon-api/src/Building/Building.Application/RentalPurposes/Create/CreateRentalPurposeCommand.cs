using Core.Application.Abstractions.Messaging;
using Core.Domain.Languages;

namespace Building.Application.RentalPurposes.Create;

public sealed record CreateRentalPurposeCommand(List<LanguageValue> Translates) : ICommand<Guid>;
