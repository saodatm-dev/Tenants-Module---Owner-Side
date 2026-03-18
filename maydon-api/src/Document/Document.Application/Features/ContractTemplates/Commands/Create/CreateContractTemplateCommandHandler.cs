using System.Text.Json;
using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Document.Application.Abstractions.Data;
using Document.Application.Features.ContractTemplates.Mappings;
using Document.Application.Features.ContractTemplates.Rendering;
using Document.Contract.ContractTemplates.Commands;
using Document.Contract.ContractTemplates.Responses;
using Document.Domain.ContractTemplates;
using Microsoft.Extensions.Logging;

namespace Document.Application.Features.ContractTemplates.Commands.Create;

public sealed class CreateContractTemplateCommandHandler(
    IDocumentDbContext db,
    IExecutionContextProvider executionContext,
    ISharedViewLocalizer sharedViewLocalizer,
    ILogger<CreateContractTemplateCommandHandler> logger)
    : ICommandHandler<CreateContractTemplateCommand, ContractTemplateResponse>
{
    public async Task<Result<ContractTemplateResponse>> Handle(
        CreateContractTemplateCommand command,
        CancellationToken cancellationToken)
    {
        // Validate blocks in each language body
        var bodiesJson = command.Bodies.RootElement;
        var validator = new BlockValidator();

        if (bodiesJson.ValueKind == JsonValueKind.Object)
        {
            foreach (var lang in bodiesJson.EnumerateObject())
            {
                if (!validator.ValidateBody(lang.Value))
                {
                    return Result.Failure<ContractTemplateResponse>(
                        sharedViewLocalizer.ContractTemplateInvalidBlocks(
                            nameof(command.Bodies), string.Join("; ", validator.Errors)));
                }
            }
        }

        // Validate theme
        if (!validator.ValidateTheme(command.Theme.RootElement))
        {
            return Result.Failure<ContractTemplateResponse>(
                sharedViewLocalizer.ContractTemplateInvalidTheme(
                    nameof(command.Theme), string.Join("; ", validator.Errors)));
        }

        var template = new ContractTemplate(
            id: Guid.NewGuid(),
            code: command.Code,
            name: command.Name.RootElement.GetRawText(),
            page: command.Page.RootElement.GetRawText(),
            theme: command.Theme.RootElement.GetRawText(),
            bodies: command.Bodies.RootElement.GetRawText())
        {
            TenantId = executionContext.TenantId,
            CreatedByUserId = executionContext.UserId,
            Scope = command.Scope.ToDomain(),
            Category = command.Category.ToDomain(),
            Description = command.Description?.RootElement.GetRawText(),
            Header = command.Header?.RootElement.GetRawText(),
            Footer = command.Footer?.RootElement.GetRawText(),
            ManualFields = command.ManualFields?.RootElement.GetRawText(),
        };

        db.ContractTemplates.Add(template);
        await db.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Created contract template {TemplateId} ({Code}) by user {UserId}",
            template.Id, template.Code, executionContext.UserId);

        return Result.Success(template.ToResponse());
    }
}
