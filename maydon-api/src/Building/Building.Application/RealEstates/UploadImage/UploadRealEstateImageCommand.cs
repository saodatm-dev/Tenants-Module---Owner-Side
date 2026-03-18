using Core.Application.Abstractions.Messaging;

namespace Building.Application.RealEstates.UploadImage;

public sealed record UploadRealEstateImageCommand(
	Guid RealEstateId,
	IReadOnlyList<string> Images) : ICommand;
