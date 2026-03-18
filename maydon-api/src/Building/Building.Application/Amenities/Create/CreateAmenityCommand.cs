using Core.Application.Abstractions.Messaging;
using Core.Domain.Languages;

namespace Building.Application.Amenities.Create;

public sealed record CreateAmenityCommand(
	Guid AmenityCategoryId,
	string IconUrl,
	List<LanguageValue> Translates) : ICommand<Guid>;
