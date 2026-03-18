using System.Text.Json;
using Core.Application.Abstractions.Messaging;
using Didox.Application.Contracts.DidoxDocuments.Queries;
using Maydon.Host.Abstractions;
using Maydon.Host.Infrastructure;

namespace Maydon.Host.Endpoints.Didox;

/// <summary>
/// Endpoints for Didox document retrieval operations
/// </summary>
internal sealed class DidoxDocumentEndpoint : IEndpoint
{
    string IEndpoint.GroupName => "didox";
    string IEndpoint.Route => "didox-documents";

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
            .WithName("GetDocumentHtml")
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
            .WithName("GetDocumentPdf")
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
            .WithName("GetDocumentJson")
            .WithSummary("Retrieve raw document JSON")
            .Produces(StatusCodes.Status200OK, contentType: "application/json")
            .Produces(StatusCodes.Status400BadRequest);
    }
}
