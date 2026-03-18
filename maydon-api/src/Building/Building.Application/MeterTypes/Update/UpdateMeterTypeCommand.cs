using Core.Application.Abstractions.Messaging;
using Core.Domain.Languages;

namespace Building.Application.MeterTypes.Update;

public sealed record UpdateMeterTypeCommand(
	Guid Id,
	IEnumerable<LanguageValue>? Names,
	IEnumerable<LanguageValue>? Description,
	string? Icon = null) : ICommand<Guid>;
