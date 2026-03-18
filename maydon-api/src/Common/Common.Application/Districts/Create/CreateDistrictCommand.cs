using Core.Application.Abstractions.Messaging;
using Core.Domain.Languages;

namespace Common.Application.Districts.Create;

public sealed record CreateDistrictCommand(
	Guid RegionId,
	List<LanguageValue> Translates) : ICommand<Guid>;
