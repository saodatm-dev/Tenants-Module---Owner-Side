using Common.Application.Languages.Get;
using Common.Domain;
using Core.Application.Abstractions.Messaging;
using Core.Application.Pagination;
using Maydon.Host.Abstractions;
using Maydon.Host.Extensions;
using Maydon.Host.Infrastructure;

namespace Maydon.Host.Endpoints.Common;

internal sealed class Languages : IEndpoint
{
	string IEndpoint.GroupName => AssemblyReference.Instance;
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapGet("/", async (
			[AsParameters] GetLanguagesQuery query,
			IQueryHandler<GetLanguagesQuery, PagedList<GetLanguagesResponse>> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(query, cancellationToken);

			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.AllowAnonymous()
			.Produces<PagedList<GetLanguagesResponse>>()
			.Produces(StatusCodes.Status400BadRequest);
	}
}
