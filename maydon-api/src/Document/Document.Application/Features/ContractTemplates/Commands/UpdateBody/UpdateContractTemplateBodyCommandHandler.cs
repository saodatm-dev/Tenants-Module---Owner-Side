using System.Text.Json;
using System.Text.Json.Nodes;
using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Messaging;
using Core.Domain.Results;
using Document.Application.Abstractions.Data;
using Document.Application.Features.ContractTemplates.Rendering;
using Document.Contract.ContractTemplates.Commands;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Document.Application.Features.ContractTemplates.Commands.UpdateBody;

public sealed class UpdateContractTemplateBodyCommandHandler(
    IDocumentDbContext db,
    IExecutionContextProvider executionContext,
    ILogger<UpdateContractTemplateBodyCommandHandler> logger)
    : ICommandHandler<UpdateContractTemplateBodyCommand>
{
    public async Task<Result> Handle(
        UpdateContractTemplateBodyCommand command,
        CancellationToken cancellationToken)
    {
        var template = await db.ContractTemplates
            .FirstOrDefaultAsync(t => t.Id == command.Id &&
                                      t.TenantId == executionContext.TenantId,
                cancellationToken);

        if (template is null)
        {
            return Result.Failure(Error.NotFound(
                "ContractTemplate.NotFound",
                $"Contract template {command.Id} not found."));
        }

        // Validate the new blocks
        var validator = new BlockValidator();
        if (!validator.ValidateBody(command.Blocks.RootElement))
        {
            return Result.Failure(Error.Validation(
                "ContractTemplate.InvalidBlocks",
                $"Invalid blocks: {string.Join("; ", validator.Errors)}"));
        }

        // Parse existing bodies, update the specific language, and serialize back
        var bodiesNode = JsonNode.Parse(template.Bodies) ?? new JsonObject();
        bodiesNode[command.Language] = JsonNode.Parse(command.Blocks.RootElement.GetRawText());

        template.Bodies = bodiesNode.ToJsonString();
        template.CurrentVersion++;
        template.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Updated body for language '{Language}' in contract template {TemplateId} by user {UserId}",
            command.Language, template.Id, executionContext.UserId);

        return Result.Success();
    }
}
