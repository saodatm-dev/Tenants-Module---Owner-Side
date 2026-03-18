using Core.Application.Abstractions.Messaging;
using Core.Domain.Languages;

namespace Common.Application.Regions.Create;

public sealed record CreateRegionCommand(List<LanguageValue> Translates) : ICommand<Guid>;
