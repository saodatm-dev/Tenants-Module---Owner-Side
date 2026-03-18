using Core.Application.Abstractions.Messaging;
using Core.Domain.Languages;

namespace Common.Application.Districts.Update;

public sealed record UpdateDistrictCommand(
	Guid Id,
	Guid RegionId,
	List<LanguageValue> Translates) : ICommand<Guid>;
