using Core.Application.Abstractions.Messaging;
using Core.Domain.Languages;

namespace Building.Application.MeterTypes.Create;

public sealed record CreateMeterTypeCommand(
	IEnumerable<LanguageValue>? Names,
	IEnumerable<LanguageValue>? Description,
	string? Icon = null) : ICommand<Guid>;
