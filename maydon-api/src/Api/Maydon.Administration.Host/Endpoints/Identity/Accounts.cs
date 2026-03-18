using Core.Application.Abstractions.Messaging;
using Core.Application.Pagination;
using Identity.Application.Accounts.Get;
using Maydon.Administration.Host.Abstractions;
using Maydon.Administration.Host.Extensions;
using Maydon.Administration.Host.Infrastructure;
using Maydon.Administration.Host.Permissions.Identity;

namespace Maydon.Administration.Host.Endpoints.Identity;

internal sealed class Accounts : IEndpoint
{
	string IEndpoint.GroupName => AccountPermissions.GroupName;
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapGet("/", async (
			IQueryHandler<GetAccountsQuery, PagedList<GetAccountsResponse>> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(new GetAccountsQuery(), cancellationToken);
			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.HasPermission(AccountPermissions.PermissionAccountList.PermissionName)
			.Produces<IEnumerable<GetAccountsResponse>>()
			.Produces(StatusCodes.Status400BadRequest);
	}
}
