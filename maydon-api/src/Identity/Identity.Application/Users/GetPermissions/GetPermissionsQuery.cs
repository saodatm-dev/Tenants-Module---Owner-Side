using Core.Application.Abstractions.Messaging;

namespace Identity.Application.Users.GetPermissions;

public sealed record GetPermissionsQuery : IQuery<IEnumerable<GetPermissionsResponse>>;
