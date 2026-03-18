using Core.Application.Abstractions.Messaging;
using Core.Domain.Languages;

namespace Common.Application.Regions.Update;

public sealed record UpdateRegionCommand(
	Guid Id,
	List<LanguageValue> Translates) : ICommand<Guid>;
