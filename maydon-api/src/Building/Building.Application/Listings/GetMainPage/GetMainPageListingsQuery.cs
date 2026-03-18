using Core.Application.Abstractions.Messaging;

namespace Building.Application.Listings.GetMainPage;

public sealed record GetMainPageListingsQuery : IQuery<IEnumerable<GetMainPageListingsResponse>>;
