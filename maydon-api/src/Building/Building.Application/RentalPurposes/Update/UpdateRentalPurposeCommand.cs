using Core.Application.Abstractions.Messaging;
using Core.Domain.Languages;

namespace Building.Application.RentalPurposes.Update;

public sealed record UpdateRentalPurposeCommand(Guid Id, List<LanguageValue> Translates) : ICommand<Guid>;
