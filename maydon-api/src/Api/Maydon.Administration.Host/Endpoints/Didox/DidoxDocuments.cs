using System.Text.Json;
using Core.Application.Abstractions.Messaging;
using Didox.Application.Contracts.DidoxDocuments.Queries;
using Maydon.Administration.Host.Abstractions;
using Maydon.Administration.Host.Extensions;
using Maydon.Administration.Host.Infrastructure;
using Maydon.Administration.Host.Permissions.Didox;

namespace Maydon.Administration.Host.Endpoints.Didox;

internal sealed class DidoxDocuments : IEndpoint
{
    string IEndpoint.GroupName => DidoxDocumentPermissions.GroupName;

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/{id}/html", async (
            string id,
            IQueryHandler<GetDocumentHtmlQuery, string> handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.Handle(new GetDocumentHtmlQuery(id), cancellationToken);

            if (result.IsFailure)
                return CustomResults.Problem(result);

            return Results.Content(result.Value, "text/html; charset=utf-8");
        })
            .HasPermission(DidoxDocumentPermissions.PermissionDidoxDocumentGetHtml.PermissionName)
            .WithName("Admin_GetDocumentHtml")
            .WithSummary("Retrieve document in HTML format")
            .Produces(StatusCodes.Status200OK, contentType: "text/html")
            .Produces(StatusCodes.Status400BadRequest);

        app.MapGet("/{id}/pdf", async (
            string id,
            IQueryHandler<GetDocumentPdfQuery, byte[]> handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.Handle(new GetDocumentPdfQuery(id), cancellationToken);

            if (result.IsFailure)
                return CustomResults.Problem(result);

            return Results.File(
                result.Value,
                contentType: "application/pdf",
                fileDownloadName: $"document-{id}.pdf");
        })
            .HasPermission(DidoxDocumentPermissions.PermissionDidoxDocumentGetPdf.PermissionName)
            .WithName("Admin_GetDocumentPdf")
            .WithSummary("Retrieve document in PDF format")
            .Produces(StatusCodes.Status200OK, contentType: "application/pdf")
            .Produces(StatusCodes.Status400BadRequest);

        app.MapGet("/{id}/json", async (
            string id,
            IQueryHandler<GetDocumentJsonQuery, JsonDocument> handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.Handle(new GetDocumentJsonQuery(id), cancellationToken);

            if (result.IsFailure)
                return CustomResults.Problem(result);

            using var jsonDoc = result.Value;
            var jsonString = jsonDoc.RootElement.GetRawText();

            return Results.Content(jsonString, "application/json; charset=utf-8");
        })
            .HasPermission(DidoxDocumentPermissions.PermissionDidoxDocumentGetJson.PermissionName)
            .WithName("Admin_GetDocumentJson")
            .WithSummary("Retrieve raw document JSON")
            .Produces(StatusCodes.Status200OK, contentType: "application/json")
            .Produces(StatusCodes.Status400BadRequest);
    }
}
