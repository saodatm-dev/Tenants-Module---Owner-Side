using Building.Domain;
using Core.Application.Abstractions.Messaging;
using Core.Application.Abstractions.Services.Minio;
using Identity.Application.Files.Download;
using Identity.Application.Files.Upload;
using Identity.Application.Files.UploadBulk;
using Maydon.Administration.Host.Abstractions;
using Maydon.Administration.Host.Extensions;
using Maydon.Administration.Host.Infrastructure;
using Maydon.Administration.Host.Permissions.Building;
using Microsoft.AspNetCore.Antiforgery;

namespace Maydon.Administration.Host.Endpoints.Building;

internal sealed class Files : IEndpoint
{
	string IEndpoint.GroupName => BuildingPermissions.GroupName;

	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapGet("/antiforgery", (IAntiforgery forgeryService, HttpContext context) =>
		{
			var tokens = forgeryService.GetAndStoreTokens(context);
			return TypedResults.Content(tokens.RequestToken!, "text/plain");
		}).AllowAnonymous();

		app.MapGet("/download/{key}", async (
			string key,
			IQueryHandler<DownloadFileQuery, FileManagerResponse> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(new DownloadFileQuery(key), cancellationToken);
			if (result.IsSuccess)
				return Results.File(result.Value.Source, result.Value.Type, result.Value.Name);

			return Results.NoContent();
		}).AllowAnonymous();

		app.MapGet("/download-url", async (
			string key,
			IFileManager fileManager,
			CancellationToken cancellationToken) =>
		{
			if (string.IsNullOrWhiteSpace(key))
				return Results.BadRequest("Key is required");

			var result = await fileManager.GetPresignedUrlAsync(Uri.UnescapeDataString(key), 3600, cancellationToken);
			if (result.IsSuccess)
				return Results.Ok(new { url = result.Value });

			return Results.NoContent();
		}).AllowAnonymous();

		app.MapPost("/upload", async (
			IFormFile file,
			ICommandHandler<UploadFileCommand, string> handler,
			CancellationToken cancellationToken) =>
		{
			var result = await handler.Handle(new UploadFileCommand(file), cancellationToken);
			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.DisableAntiforgery()
			.AllowAnonymous()
			.Produces<string>()
			.Produces(StatusCodes.Status400BadRequest);

		app.MapPost("/upload-bulk", async (
			IFormFileCollection files,
			ICommandHandler<UploadBulkFileCommand, IEnumerable<string>> handler,
			CancellationToken cancellationToken) =>
		{
			if (files is null || !files.Any())
				return Results.NoContent();

			var result = await handler.Handle(new UploadBulkFileCommand(files), cancellationToken);
			return result.Match(Results.Ok, CustomResults.Problem);
		})
			.DisableAntiforgery()
			.AllowAnonymous()
			.Produces<IEnumerable<string>>()
			.Produces(StatusCodes.Status400BadRequest);
	}
}
