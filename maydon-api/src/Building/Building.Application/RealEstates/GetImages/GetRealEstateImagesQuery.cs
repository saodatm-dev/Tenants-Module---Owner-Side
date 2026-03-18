using Core.Application.Abstractions.Messaging;

namespace Building.Application.RealEstates.GetImages;

public sealed record GetRealEstateImagesQuery(Guid RealEstateId) : IQuery<IEnumerable<GetRealEstateImagesResponse>>;
