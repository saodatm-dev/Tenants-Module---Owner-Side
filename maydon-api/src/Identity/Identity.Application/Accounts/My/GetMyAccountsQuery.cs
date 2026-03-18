using Core.Application.Abstractions.Messaging;

namespace Identity.Application.Accounts.My;

public sealed record GetMyAccountsQuery : IQuery<IEnumerable<GetMyAccountsResponse>>;
