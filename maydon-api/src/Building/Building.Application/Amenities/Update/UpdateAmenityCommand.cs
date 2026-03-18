using Core.Application.Abstractions.Messaging;
using Core.Domain.Languages;

namespace Building.Application.Amenities.Update;

public sealed record UpdateAmenityCommand(
	Guid Id,
	Guid AmenityCategoryId,
	string IconUrl,
	List<LanguageValue> Translates) : ICommand<Guid>;
