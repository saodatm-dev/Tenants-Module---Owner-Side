using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Messaging;
using Core.Domain.Results;
using Document.Application.Abstractions.Data;
using Document.Application.Features.ContractTemplates.Mappings;
using Document.Contract.ContractTemplates.Commands;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Document.Application.Features.ContractTemplates.Commands.Update;

public sealed class UpdateContractTemplateCommandHandler(
    IDocumentDbContext db,
    IExecutionContextProvider executionContext,
    ILogger<UpdateContractTemplateCommandHandler> logger)
    : ICommandHandler<UpdateContractTemplateCommand>
{
    public async Task<Result> Handle(
        UpdateContractTemplateCommand command,
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

        if (command.Code is not null) template.Code = command.Code;
        if (command.Name is not null) template.Name = command.Name.RootElement.GetRawText();
        if (command.Description is not null) template.Description = command.Description.RootElement.GetRawText();
        if (command.Page is not null) template.Page = command.Page.RootElement.GetRawText();
        if (command.Theme is not null) template.Theme = command.Theme.RootElement.GetRawText();
        if (command.Header is not null) template.Header = command.Header.RootElement.GetRawText();
        if (command.Footer is not null) template.Footer = command.Footer.RootElement.GetRawText();
        if (command.Bodies is not null) template.Bodies = command.Bodies.RootElement.GetRawText();
        if (command.ManualFields is not null) template.ManualFields = command.ManualFields.RootElement.GetRawText();
        if (command.Scope.HasValue) template.Scope = command.Scope.Value.ToDomain();
        if (command.Category.HasValue) template.Category = command.Category.Value.ToDomain();
        if (command.IsActive.HasValue) template.IsActive = command.IsActive.Value;

        template.CurrentVersion++;
        template.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Updated contract template {TemplateId} to version {Version} by user {UserId}",
            template.Id, template.CurrentVersion, executionContext.UserId);

        return Result.Success();
    }
}
