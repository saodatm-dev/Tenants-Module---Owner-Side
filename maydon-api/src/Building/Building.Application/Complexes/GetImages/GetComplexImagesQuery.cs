using Core.Application.Abstractions.Messaging;

namespace Building.Application.Complexes.GetImages;

public sealed record GetComplexImagesQuery(Guid Id) : IQuery<IEnumerable<string>>;
