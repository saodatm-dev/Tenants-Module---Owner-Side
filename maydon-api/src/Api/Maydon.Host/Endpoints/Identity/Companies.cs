using Core.Application.Abstractions.Messaging;
using Core.Application.Pagination;
using Identity.Application.Companies.Get;
using Identity.Application.Companies.GetById;
using Identity.Application.Companies.Remove;
using Identity.Application.Companies.UpdateLogo;
using Identity.Application.CompanyUsers.Get;
using Maydon.Host.Abstractions;
using Maydon.Host.Extensions;
using Maydon.Host.Infrastructure;
using Maydon.Host.Permissions.Identity;

namespace Maydon.Host.Endpoints.Identity;

internal sealed class Companies : IEndpoint
{
	string IEndpoint.GroupName => CompanyPermissions.GroupName;
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapGet("/", async (
			[AsParameters] GetCompaniesQuery query,
			IQueryHandler<GetCompaniesQuery, PagedList<GetCompaniesResponse>> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(query, cancellationToken);

			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.Produces<PagedList<GetCompaniesResponse>>()
			.Produces(StatusCodes.Status400BadRequest);

		app.MapGet("/{id:Guid}", async (
		   Guid id,
		   IQueryHandler<GetCompanyByIdQuery, GetCompanyByIdResponse> handler,
		   CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(new GetCompanyByIdQuery(id), cancellationToken);

			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.HasPermission(CompanyPermissions.PermissionCompanyGetById.PermissionName)
			.Produces<GetCompanyByIdResponse>()
			.Produces(StatusCodes.Status400BadRequest);

		//app.MapPost("/", async (
		//   CreateInvitationCommand command,
		//   ICommandHandler<CreateInvitationCommand, Guid> handler,
		//   CancellationToken cancellationToken) =>
		//{
		//	var result = await handler.Handle(command, cancellationToken);

		//	return result.Match(Results.Ok, CustomResults.Problem);
		//})
		//	.HasPermission(PermissionInvitationCreate)
		//	.Produces<Guid>()
		//	.Produces(StatusCodes.Status400BadRequest);

		app.MapPut("/logo", async (
		   UpdateCompanyLogoCommand command,
		   ICommandHandler<UpdateCompanyLogoCommand> handler,
		   CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(command, cancellationToken);

			return result.Match(CustomResults.Ok, CustomResults.Problem);
		})
			.HasPermission(CompanyPermissions.PermissionCompanyLogo.PermissionName)
			.Produces(StatusCodes.Status200OK)
			.Produces(StatusCodes.Status400BadRequest);

		app.MapDelete("/{id:guid}", async (
		   Guid id,
		   ICommandHandler<RemoveCompanyCommand, Guid> handler,
		   CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(new RemoveCompanyCommand(id), cancellationToken);

			return result.Match(CustomResults.Ok, CustomResults.Problem);
		})
			.HasPermission(CompanyPermissions.PermissionCompanyRemove.PermissionName)
			.Produces<Guid>()
			.Produces(StatusCodes.Status400BadRequest);

		app.MapGet("/users", async (
			[AsParameters] GetCompanyUsersQuery query,
			IQueryHandler<GetCompanyUsersQuery, PagedList<GetCompanyUsersResponse>> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(query, cancellationToken);

			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.Produces<PagedList<GetCompanyUsersResponse>>()
			.Produces(StatusCodes.Status400BadRequest);
	}
}

