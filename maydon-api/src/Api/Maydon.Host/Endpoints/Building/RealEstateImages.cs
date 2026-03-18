using Building.Application.RealEstates.GetImages;
using Building.Application.RealEstates.RemoveImage;
using Building.Application.RealEstates.UploadImage;
using Core.Application.Abstractions.Messaging;
using Maydon.Host.Abstractions;
using Maydon.Host.Extensions;
using Maydon.Host.Infrastructure;
using Maydon.Host.Permissions.Building;

namespace Maydon.Host.Endpoints.Building;

internal sealed class RealEstateImages : IEndpoint
{
	string IEndpoint.GroupName => RealEstateImagePermissions.GroupName;

	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapGet("/{realEstateId:guid}", async (
			Guid realEstateId,
			IQueryHandler<GetRealEstateImagesQuery, IEnumerable<GetRealEstateImagesResponse>> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(new GetRealEstateImagesQuery(realEstateId), cancellationToken);
			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.AllowAnonymous()
			.Produces<IEnumerable<GetRealEstateImagesResponse>>()
			.Produces(StatusCodes.Status400BadRequest);

		app.MapPost("/upload/{realEstateId:guid}", async (
			UploadRealEstateImageCommand command,
			ICommandHandler<UploadRealEstateImageCommand> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(command, cancellationToken);
			return result.Match(CustomResults.Ok, CustomResults.Problem);
		})
			.HasPermission(RealEstateImagePermissions.PermissionRealEstateImageUpload.PermissionName)
			.Produces<List<Guid>>()
			.Produces(StatusCodes.Status400BadRequest);

		app.MapDelete("/{id:guid}", async (
			Guid id,
			ICommandHandler<RemoveRealEstateImageCommand> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(new RemoveRealEstateImageCommand(id), cancellationToken);
			return result.Match(Results.NoContent, CustomResults.Problem);
		})
			.HasPermission(RealEstateImagePermissions.PermissionRealEstateImageRemove.PermissionName)
			.Produces(StatusCodes.Status204NoContent)
			.Produces(StatusCodes.Status400BadRequest);
	}
}
